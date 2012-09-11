#region License

// Copyright 2004-2012 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;
using NUnit.Framework;
using Quokka.NH.Implementations;
using Quokka.NH.Startup;
using Quokka.NH.Tests.Model;
using Quokka.NH.Tests.Support;

// ReSharper disable InconsistentNaming

namespace Quokka.NH.Tests
{
	[TestFixture]
	public class SessionManagerTests
	{
		private IWindsorContainer _container;
		private ISessionManager _sessionManager;

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			_container = new WindsorContainer();
			_container.AddFacility<NHibernateFacility>(f => f.DefaultAlias = TestConfigurationBuilder.Alias);
			_container.Register(
				Component.For<IConfigurationBuilder, ISessionFactoryContributor>()
					.ImplementedBy<TestConfigurationBuilder>());

			_sessionManager = _container.Resolve<ISessionManager>();
			DeleteAllBlogs();
		}

		[TestFixtureTearDown]
		public void TestFixtureTearDown()
		{
			DeleteAllBlogs();
			_container.Dispose();
			_container = null;
			_sessionManager = null;
		}

		[Test]
		public void Simple_use_with_default_alias()
		{
			using (var session = _sessionManager.OpenSession())
			{
				var blogs = session.QueryOver<Blog>().List();
				Assert.AreEqual(0, blogs.Count);
			}
		}

		[Test]
		public void Nested_sessions()
		{
			using (var session1 = _sessionManager.OpenSession())
			{
				using (var session2 = _sessionManager.OpenSession())
				{
					using (var session3 = _sessionManager.OpenSession(TestConfigurationBuilder.Alias))
					{
						var sessionDelegate1 = (SessionDelegate) session1;
						var sessionDelegate2 = (SessionDelegate) session2;
						var sessionDelegate3 = (SessionDelegate) session3;

						Assert.AreSame(sessionDelegate1.InnerSession, sessionDelegate2.InnerSession);
						Assert.AreSame(sessionDelegate1.InnerSession, sessionDelegate3.InnerSession);
						Assert.AreSame(sessionDelegate2.InnerSession, sessionDelegate3.InnerSession);
					}
				}
			}
		}

		[Test]
		public void Nested_sessions_and_transactions()
		{
			var tx3Committed = false;
			var tx2Committed = false;
			var tx1Committed = false;

			using (var session1 = _sessionManager.OpenSession())
			{
				using (var tx1 = session1.BeginTransaction())
				{
					session1.AfterCommit(() => tx1Committed = true);
					using (var session2 = _sessionManager.OpenSession())
					{
						using (var tx2 = session2.BeginTransaction())
						{
							session2.AfterCommit(() => tx2Committed = true);
							using (var session3 = _sessionManager.OpenSession(TestConfigurationBuilder.Alias))
							{
								using (var tx3 = session3.BeginTransaction())
								{
									session3.AfterCommit(() => tx3Committed = true);

									var sessionDelegate1 = (SessionDelegate) session1;
									var sessionDelegate2 = (SessionDelegate) session2;
									var sessionDelegate3 = (SessionDelegate) session3;

									Assert.AreSame(sessionDelegate1.InnerSession, sessionDelegate2.InnerSession);
									Assert.AreSame(sessionDelegate1.InnerSession, sessionDelegate3.InnerSession);
									Assert.AreSame(sessionDelegate2.InnerSession, sessionDelegate3.InnerSession);

									tx3.Commit();
									Assert.IsFalse(tx3Committed);
								}
							}
							tx2.Commit();
							Assert.IsFalse(tx2Committed);
						}
					}
					tx1.Commit();
					Assert.IsTrue(tx1Committed);
					Assert.IsTrue(tx2Committed);
					Assert.IsTrue(tx3Committed);
				}
			}
		}

		[Test]
		public void Implicit_rollback_in_nested_transaction()
		{
			using (var session1 = _sessionManager.OpenSession())
			{
				using (var tx1 = session1.BeginTransaction())
				{
					using (var session2 = _sessionManager.OpenSession())
					{
						// ReSharper disable UnusedVariable
						using (var tx2 = session2.BeginTransaction())
						{
							// do not commit, will cause an implicit rollback
						}
						// ReSharper restore UnusedVariable
					}

					// An attempt to commit the transaction will result in an exception.
					// We don't use Assert.Throws here because it complains about using
					// a disposed object in the closure.
					try
					{
						tx1.Commit();
						Assert.Fail("Expected Exception");
					}
					catch (TransactionException)
					{
					}
				}
			}
		}

		[Test]
		public void Explicit_rollback_in_nested_transaction()
		{
			using (var session1 = _sessionManager.OpenSession())
			{
				using (var tx1 = session1.BeginTransaction())
				{
					var b1 = new Blog();
					session1.Save(b1);

					using (var session2 = _sessionManager.OpenSession())
					{
						using (var tx2 = session2.BeginTransaction())
						{
							var b2 = new Blog();
							session2.Save(b2);

							// explicit rollback
							tx2.Rollback();
						}
					}

					var b3 = new Blog();
					session1.Save(b3);

					// ReSharper disable AccessToDisposedClosure
					// ReSharper disable ConvertClosureToMethodGroup
					Assert.Throws<TransactionException>(() => tx1.Commit());
					// ReSharper restore ConvertClosureToMethodGroup
					// ReSharper restore AccessToDisposedClosure
				}
			}

			// verify that nothing was written to the database
			using (var session = _sessionManager.OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					var blogs = session.QueryOver<Blog>().List();
					try
					{
						Assert.AreEqual(0, blogs.Count, "Expected database to be unchanged");
					}
					finally
					{
						foreach (var blog in blogs)
						{
							session.Delete(blog);
						}
						tx.Commit();
					}
				}
			}
		}

		[Test]
		public void Rollback_followed_by_nested_transaction()
		{
			using (var session = _sessionManager.OpenSession())
			{
				// First transaction rolls back
				using (var tx1 = session.BeginTransaction())
				{
					var blog1 = new Blog {Name = "One"};
					session.Save(blog1);
					session.Flush();
					tx1.Rollback();
				}

				// Second session commits
				using (var tx2 = session.BeginTransaction())
				{
					var blogs = session.QueryOver<Blog>().List();
					Assert.AreEqual(0, blogs.Count);

					// commit a transaction, should not throw
					tx2.Commit();
				}
			}

		}

		[Test]
		public void Nested_stateless_sessions_are_different()
		{
			using (var session1 = _sessionManager.OpenStatelessSession())
			{
				using (var session2 = _sessionManager.OpenStatelessSession())
				{
					using (var session3 = _sessionManager.OpenStatelessSession(TestConfigurationBuilder.Alias))
					{
						var sessionId1 = session1.GetSessionImplementation().SessionId;
						var sessionId2 = session2.GetSessionImplementation().SessionId;
						var sessionId3 = session3.GetSessionImplementation().SessionId;

						Assert.AreNotEqual(sessionId1, sessionId2);
						Assert.AreNotEqual(sessionId1, sessionId3);
						Assert.AreNotEqual(sessionId2, sessionId3);
					}
				}
			}
		}

		private void DeleteAllBlogs()
		{
			using (var session = _sessionManager.OpenSession())
			{
				using (var tx = session.BeginTransaction())
				{
					var blogs = session.QueryOver<Blog>().List();
					foreach (var blog in blogs)
					{
						session.Delete(blog);
					}
					tx.Commit();
				}
			}
		}
	}
}