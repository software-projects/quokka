﻿#region License

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

using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Quokka.NH.Tests.Model;
using Quokka.NH.Tests.Support;

// ReSharper disable InconsistentNaming
namespace Quokka.NH.Tests
{
	/// <summary>
	/// Tests access to database without any windsor container or facility config.
	/// </summary>
	/// <remarks>
	/// The purpose of this test fixture is to verify that the SQLite test database
	/// code is working correctly.
	/// </remarks>
	[TestFixture]
	public class SqliteTestDatabaseTests
	{
		protected ISession Session { get; private set; }
		protected ISessionFactory SessionFactory { get; private set; }
		protected Configuration Configuration { get; private set; }
		protected TestConfigurationBuilder NHibernateInstaller { get; private set; }

		[SetUp]
		public void SetUp()
		{
			NHibernateInstaller = new TestConfigurationBuilder();
			Configuration = NHibernateInstaller.BuildConfiguration("testdb");
			SessionFactory = Configuration.BuildSessionFactory();
			NHibernateInstaller.Contribute("testdb", SessionFactory, Configuration);
			Session = SessionFactory.OpenSession();
		}

		[TearDown]
		public void TearDown()
		{
			if (Session != null)
			{
				Session.Dispose();
				Session = null;
			}

			if (SessionFactory != null)
			{
				SessionFactory.Dispose();
				SessionFactory = null;
			}

			if (NHibernateInstaller != null)
			{
				NHibernateInstaller.Dispose();
				NHibernateInstaller = null;
			}

			Configuration = null;
		}

		[Test]
		public void Can_create_rows()
		{
			var b1 = new Blog
			         	{
			         		Name = "Blog 1",
			         	};
			Session.Save(b1);
			Session.Flush();

			Assert.AreEqual(1, Session.QueryOver<Blog>().List().Count);

			Session.Delete(b1);
			Session.Flush();

			Assert.AreEqual(0, Session.QueryOver<Blog>().List().Count);
		}
	}
}
