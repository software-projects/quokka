#region License

// Copyright 2004-2014 John Jeffery
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
using System.Threading;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Quokka.Threading
{
	[TestFixture]
	public class WorkerActionTests
	{
		[Test]
		public void DoWork_with_no_WhenComplete()
		{
			bool doWorkDone = false;

			var worker = new Worker();

			WorkerAction.Define()
				.DoWork(delegate { doWorkDone = true; })
				.Run(worker);

			Assert.IsTrue(doWorkDone, "work not done");
		}

		[Test]
		public void DoWork_with_WhenComplete()
		{
			bool doWorkDone = false;
			bool completed = false;

			var worker = new Worker();

			WorkerAction.Define()
				.DoWork(delegate { doWorkDone = true; })
				.WhenComplete(() => completed = true)
				.Run(worker);

			Assert.IsTrue(doWorkDone, "work not done");
			Assert.IsTrue(completed, "not completed");
		}

		[Test]
		public void DoWork_returns_result_and_passes_to_WhenComplete()
		{
			int result = 0;

			var action = WorkerAction.Define()
				.DoWork(() => 2 + 2)
				.WhenComplete(n => result = n)
				.Create();

			action.Run();
			Assert.AreEqual(4, result);
		}

		[Test]
		public void Calls_WhenError_when_exception_is_thrown()
		{
			bool whenCompleteCalled = false;
			Exception exception = null;

			var action = WorkerAction.Define()
				.DoWork(delegate { throw new ApplicationException("Test exception"); })
				.WhenComplete(() => whenCompleteCalled = true)
				.WhenError(ex => exception = ex)
				.Create();

			action.Run();
			Assert.IsFalse(whenCompleteCalled);
			Assert.IsNotNull(exception);
			Assert.IsInstanceOf<ApplicationException>(exception);
			Assert.AreEqual("Test exception", exception.Message);
		}

		[Test]
		public void Calls_WhenCanceled_when_OperationCanceledException_is_thrown()
		{
			bool whenCompleteCalled = false;
			bool whenCanceledCalled = false;
			Exception exception = null;

			var action = WorkerAction.Define()
				.DoWork(delegate { throw new OperationCanceledException("Test exception"); })
				.WhenComplete(() => whenCompleteCalled = true)
				.WhenCanceled(() => whenCanceledCalled = true)
				.WhenError(ex => exception = ex)
				.Create();

			action.Run();
			Assert.IsFalse(whenCompleteCalled);
			Assert.IsNull(exception);
			Assert.IsTrue(whenCanceledCalled);
		}

		[Test]
		public void Calls_WhenComplete_via_SynchronizationContext()
		{
			var sc = new TestSynchronizationContext();
			bool doWorkWasRun = false;
			bool whenCompleteWasRun = false;

			var action = WorkerAction.Define()
				.DoWork(delegate
				{
					// Verifies that this is not being called
					// via the SynchronizationContext
					Assert.IsFalse(sc.IsInSend);
					doWorkWasRun = true;
					return 2 + 2;
				})
				.WhenComplete(delegate(int n)
				{
					Assert.IsTrue(sc.IsInSend);
					Assert.AreEqual(4, n);
					whenCompleteWasRun = true;
				})
				.WhenError(ex => Assert.Fail("When error called"))
				.Create();


			action.SynchronizationContext = sc;

			action.Run();

			Assert.IsTrue(doWorkWasRun);
			Assert.IsTrue(whenCompleteWasRun);
		}

		[Test]
		public void Calls_WhenError_via_SynchronizationContext()
		{
			var sc = new TestSynchronizationContext();
			var whenErrorWasCalled = false;

			var workerAction = WorkerAction.Define()
				.DoWork(delegate
				        	{
				        		throw new ApplicationException("Test exception");
				        	})
				.WhenError(delegate
				           	{
				           		Assert.IsTrue(sc.IsInSend);
				           		whenErrorWasCalled = true;
				           	})
				.Create();

			workerAction.SynchronizationContext = sc;
			workerAction.Run();

			Assert.IsTrue(whenErrorWasCalled, "WhenError was not called");
		}

		[Test]
        public void Calls_module_functions_when_successful_completion()
		{
			int callOrder = 0;
			int module1BeforeCalled = 0;
			int module1AfterCalled = 0;
			int module1ErrorCalled = 0;
			Exception module1Error = null;
			int module1FinishedCalled = 0;

			var module1 = new TestModule
			              	{
			              		BeforeCallback = delegate { module1BeforeCalled = ++callOrder; },
			              		AfterCallback = delegate { module1AfterCalled = ++callOrder; },
								ErrorCallback = delegate(Exception ex)
								                	{
								                		module1ErrorCalled = ++callOrder; 
														module1Error = ex; 
													},
			              		FinishedCallback = delegate { module1FinishedCalled = ++callOrder; },
			              	};

			var action = WorkerAction.Define()
				.DoWork(delegate { })
				.AddModule(module1)
				.Create();

			action.Run();

			Assert.AreEqual(1, module1BeforeCalled);
			Assert.AreEqual(2, module1AfterCalled);
			Assert.AreEqual(0, module1ErrorCalled);
			Assert.IsNull(module1Error);
			Assert.AreEqual(3, module1FinishedCalled);
		}

		[Test]
		public void Calls_multiple_module_functions_when_successful_completion()
		{
			int callOrder = 0;
			int module1BeforeCalled = 0;
			int module1AfterCalled = 0;
			int module1ErrorCalled = 0;
			Exception module1Error = null;
			int module1FinishedCalled = 0;

			int module2BeforeCalled = 0;
			int module2AfterCalled = 0;
			int module2ErrorCalled = 0;
			Exception module2Error = null;
			int module2FinishedCalled = 0;

			int module3BeforeCalled = 0;
			int module3AfterCalled = 0;
			int module3ErrorCalled = 0;
			Exception module3Error = null;
			int module3FinishedCalled = 0;

			var module1 = new TestModule
			{
				BeforeCallback = delegate { module1BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module1AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module1ErrorCalled = ++callOrder;
					module1Error = ex;
				},
				FinishedCallback = delegate { module1FinishedCalled = ++callOrder; },
			};

			var module2 = new TestModule
			{
				BeforeCallback = delegate { module2BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module2AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module2ErrorCalled = ++callOrder;
					module2Error = ex;
				},
				FinishedCallback = delegate { module2FinishedCalled = ++callOrder; },
			};

			var module3 = new TestModule
			{
				BeforeCallback = delegate { module3BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module3AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module3ErrorCalled = ++callOrder;
					module3Error = ex;
				},
				FinishedCallback = delegate { module3FinishedCalled = ++callOrder; },
			};

			var action = WorkerAction.Define()
				.DoWork(delegate { })
				.AddModule(module1)
				.AddModule(module2)
				.AddModule(module3)
				.Create();

			action.Run();

			Assert.AreEqual(1, module1BeforeCalled);
			Assert.AreEqual(2, module2BeforeCalled);
			Assert.AreEqual(3, module3BeforeCalled);
			Assert.AreEqual(4, module3AfterCalled);
			Assert.AreEqual(5, module2AfterCalled);
			Assert.AreEqual(6, module1AfterCalled);
			Assert.AreEqual(0, module3ErrorCalled);
			Assert.IsNull(module3Error);
			Assert.AreEqual(0, module2ErrorCalled);
			Assert.IsNull(module2Error);
			Assert.AreEqual(0, module1ErrorCalled);
			Assert.IsNull(module1Error);
			Assert.AreEqual(7, module3FinishedCalled);
			Assert.AreEqual(8, module2FinishedCalled);
			Assert.AreEqual(9, module1FinishedCalled);
		}

		[Test]
		public void Calls_multiple_module_functions_when_exception_thrown_by_DoWork()
		{
			int callOrder = 0;
			Exception exceptionThrown = null;

			int module1BeforeCalled = 0;
			int module1AfterCalled = 0;
			int module1ErrorCalled = 0;
			Exception module1Error = null;
			int module1FinishedCalled = 0;

			int module2BeforeCalled = 0;
			int module2AfterCalled = 0;
			int module2ErrorCalled = 0;
			Exception module2Error = null;
			int module2FinishedCalled = 0;

			int module3BeforeCalled = 0;
			int module3AfterCalled = 0;
			int module3ErrorCalled = 0;
			Exception module3Error = null;
			int module3FinishedCalled = 0;

			var module1 = new TestModule
			{
				BeforeCallback = delegate { module1BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module1AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module1ErrorCalled = ++callOrder;
					module1Error = ex;
				},
				FinishedCallback = delegate { module1FinishedCalled = ++callOrder; },
			};

			var module2 = new TestModule
			{
				BeforeCallback = delegate { module2BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module2AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module2ErrorCalled = ++callOrder;
					module2Error = ex;
				},
				FinishedCallback = delegate { module2FinishedCalled = ++callOrder; },
			};

			var module3 = new TestModule
			{
				BeforeCallback = delegate { module3BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module3AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module3ErrorCalled = ++callOrder;
					module3Error = ex;
				},
				FinishedCallback = delegate { module3FinishedCalled = ++callOrder; },
			};

			var action = WorkerAction.Define()
				.DoWork(delegate
				        	{
				        		exceptionThrown = new ApplicationException("Test exception");
				        		throw exceptionThrown;
				        	})
				.AddModule(module1)
				.AddModule(module2)
				.AddModule(module3)
				.Create();

			action.Run();

			Assert.AreEqual(1, module1BeforeCalled);
			Assert.AreEqual(2, module2BeforeCalled);
			Assert.AreEqual(3, module3BeforeCalled);
			Assert.AreEqual(0, module3AfterCalled);
			Assert.AreEqual(0, module2AfterCalled);
			Assert.AreEqual(0, module1AfterCalled);
			Assert.AreEqual(4, module3ErrorCalled);
			Assert.AreSame(exceptionThrown, module3Error);
			Assert.AreEqual(5, module2ErrorCalled);
			Assert.AreSame(exceptionThrown, module2Error);
			Assert.AreEqual(6, module1ErrorCalled);
			Assert.AreSame(exceptionThrown, module1Error);
			Assert.AreEqual(7, module3FinishedCalled);
			Assert.AreEqual(8, module2FinishedCalled);
			Assert.AreEqual(9, module1FinishedCalled);
		}

		[Test]
		public void Calls_multiple_module_functions_when_exception_thrown_by_Module_before()
		{
			int callOrder = 0;
			Exception exceptionThrown = null;

			int module1BeforeCalled = 0;
			int module1AfterCalled = 0;
			int module1ErrorCalled = 0;
			Exception module1Error = null;
			int module1FinishedCalled = 0;

			int module2BeforeCalled = 0;
			int module2AfterCalled = 0;
			int module2ErrorCalled = 0;
			Exception module2Error = null;
			int module2FinishedCalled = 0;

			int module3BeforeCalled = 0;
			int module3AfterCalled = 0;
			int module3ErrorCalled = 0;
			Exception module3Error = null;
			int module3FinishedCalled = 0;

			bool doWorkCalled = false;

			var module1 = new TestModule
			{
				BeforeCallback = delegate { module1BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module1AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module1ErrorCalled = ++callOrder;
					module1Error = ex;
				},
				FinishedCallback = delegate { module1FinishedCalled = ++callOrder; },
			};

			var module2 = new TestModule
			{
				BeforeCallback = delegate
				                 	{
				                 		module2BeforeCalled = ++callOrder;
				                 		exceptionThrown = new ApplicationException("thrown by module");
				                 		throw exceptionThrown;
				                 	},
				AfterCallback = delegate { module2AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module2ErrorCalled = ++callOrder;
					module2Error = ex;
				},
				FinishedCallback = delegate { module2FinishedCalled = ++callOrder; },
			};

			var module3 = new TestModule
			{
				BeforeCallback = delegate { module3BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module3AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module3ErrorCalled = ++callOrder;
					module3Error = ex;
				},
				FinishedCallback = delegate { module3FinishedCalled = ++callOrder; },
			};

			var action = WorkerAction.Define()
				.DoWork(delegate { doWorkCalled = true; })
				.AddModule(module1)
				.AddModule(module2)
				.AddModule(module3)
				.Create();

			action.Run();

			Assert.AreEqual(1, module1BeforeCalled);
			Assert.AreEqual(2, module2BeforeCalled);
			Assert.AreEqual(0, module3BeforeCalled);
			Assert.AreEqual(0, module3AfterCalled);
			Assert.AreEqual(0, module2AfterCalled);
			Assert.AreEqual(0, module1AfterCalled);
			Assert.AreEqual(0, module3ErrorCalled); 
			Assert.IsNull(module3Error);
			Assert.AreEqual(0, module2ErrorCalled);
			Assert.IsNull(module2Error);
			Assert.AreEqual(3, module1ErrorCalled);
			Assert.AreSame(exceptionThrown, module1Error);
			Assert.AreEqual(0, module3FinishedCalled);
			Assert.AreEqual(0, module2FinishedCalled);
			Assert.AreEqual(4, module1FinishedCalled);
			Assert.IsFalse(doWorkCalled);
		}


		[Test]
		public void Calls_multiple_module_functions_when_exception_thrown_by_Module_after()
		{
			int callOrder = 0;
			Exception exceptionThrown = null;

			int module1BeforeCalled = 0;
			int module1AfterCalled = 0;
			int module1ErrorCalled = 0;
			Exception module1Error = null;
			int module1FinishedCalled = 0;

			int module2BeforeCalled = 0;
			int module2AfterCalled = 0;
			int module2ErrorCalled = 0;
			Exception module2Error = null;
			int module2FinishedCalled = 0;

			int module3BeforeCalled = 0;
			int module3AfterCalled = 0;
			int module3ErrorCalled = 0;
			Exception module3Error = null;
			int module3FinishedCalled = 0;

			bool doWorkCalled = false;

			var module1 = new TestModule
			{
				BeforeCallback = delegate { module1BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module1AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module1ErrorCalled = ++callOrder;
					module1Error = ex;
				},
				FinishedCallback = delegate { module1FinishedCalled = ++callOrder; },
			};

			var module2 = new TestModule
			{
				BeforeCallback = delegate
				{
					module2BeforeCalled = ++callOrder;
				},
				AfterCallback = delegate
				                	{
				                		module2AfterCalled = ++callOrder;
										exceptionThrown = new ApplicationException("thrown by module");
										throw exceptionThrown;
				                	},
				ErrorCallback = delegate(Exception ex)
				{
					module2ErrorCalled = ++callOrder;
					module2Error = ex;
				},
				FinishedCallback = delegate { module2FinishedCalled = ++callOrder; },
			};

			var module3 = new TestModule
			{
				BeforeCallback = delegate { module3BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module3AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module3ErrorCalled = ++callOrder;
					module3Error = ex;
				},
				FinishedCallback = delegate { module3FinishedCalled = ++callOrder; },
			};

			var action = WorkerAction.Define()
				.DoWork(delegate { doWorkCalled = true; })
				.AddModule(module1)
				.AddModule(module2)
				.AddModule(module3)
				.Create();

			action.Run();

			Assert.AreEqual(1, module1BeforeCalled);
			Assert.AreEqual(2, module2BeforeCalled);
			Assert.AreEqual(3, module3BeforeCalled);
			Assert.AreEqual(4, module3AfterCalled);
			Assert.AreEqual(5, module2AfterCalled);
			Assert.AreEqual(0, module1AfterCalled);
			Assert.AreEqual(0, module3ErrorCalled); 
			Assert.IsNull(module3Error);
			Assert.AreEqual(0, module2ErrorCalled);
			Assert.IsNull(module2Error);
			Assert.AreEqual(6, module1ErrorCalled);
			Assert.AreSame(exceptionThrown, module1Error);
			Assert.AreEqual(7, module3FinishedCalled);
			Assert.AreEqual(8, module2FinishedCalled);
			Assert.AreEqual(9, module1FinishedCalled);
			Assert.IsTrue(doWorkCalled);
		}


		[Test]
		public void Calls_multiple_module_functions_when_exception_thrown_by_Module_error()
		{
			int callOrder = 0;
			Exception exceptionThrown = null;
			Exception errorExceptionThrown = null;

			int module1BeforeCalled = 0;
			int module1AfterCalled = 0;
			int module1ErrorCalled = 0;
			Exception module1Error = null;
			int module1FinishedCalled = 0;

			int module2BeforeCalled = 0;
			int module2AfterCalled = 0;
			int module2ErrorCalled = 0;
			Exception module2Error = null;
			int module2FinishedCalled = 0;

			int module3BeforeCalled = 0;
			int module3AfterCalled = 0;
			int module3ErrorCalled = 0;
			Exception module3Error = null;
			int module3FinishedCalled = 0;

			var module1 = new TestModule
			{
				BeforeCallback = delegate { module1BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module1AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module1ErrorCalled = ++callOrder;
					module1Error = ex;
				},
				FinishedCallback = delegate { module1FinishedCalled = ++callOrder; },
			};

			var module2 = new TestModule
			{
				BeforeCallback = delegate
				{
					module2BeforeCalled = ++callOrder;
				},
				AfterCallback = delegate
				{
					module2AfterCalled = ++callOrder;

				},
				ErrorCallback = delegate(Exception ex)
				{
					module2ErrorCalled = ++callOrder;
					module2Error = ex;
					errorExceptionThrown = new ApplicationException("thrown by module");
					throw errorExceptionThrown;
				},
				FinishedCallback = delegate { module2FinishedCalled = ++callOrder; },
			};

			var module3 = new TestModule
			{
				BeforeCallback = delegate { module3BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module3AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module3ErrorCalled = ++callOrder;
					module3Error = ex;
				},
				FinishedCallback = delegate { module3FinishedCalled = ++callOrder; },
			};

			var action = WorkerAction.Define()
				.DoWork(delegate
				        	{
				        		exceptionThrown = new ApplicationException("Thrown by DoWork");
				        		throw exceptionThrown;
				        	})
				.AddModule(module1)
				.AddModule(module2)
				.AddModule(module3)
				.Create();

			action.Run();

			Assert.AreEqual(1, module1BeforeCalled);
			Assert.AreEqual(2, module2BeforeCalled);
			Assert.AreEqual(3, module3BeforeCalled);
			Assert.AreEqual(0, module3AfterCalled);
			Assert.AreEqual(0, module2AfterCalled);
			Assert.AreEqual(0, module1AfterCalled);
			Assert.AreEqual(4, module3ErrorCalled);
			Assert.AreSame(exceptionThrown, module3Error);
			Assert.AreEqual(5, module2ErrorCalled);
			Assert.AreSame(exceptionThrown, module2Error);
			Assert.AreEqual(6, module1ErrorCalled);
			Assert.AreSame(exceptionThrown, module1Error);
			Assert.AreEqual(7, module3FinishedCalled);
			Assert.AreEqual(8, module2FinishedCalled);
			Assert.AreEqual(9, module1FinishedCalled);
		}


		[Test]
		public void Calls_multiple_module_functions_when_exception_thrown_by_Module_finished()
		{
			int callOrder = 0;
			Exception errorExceptionThrown = null;

			int module1BeforeCalled = 0;
			int module1AfterCalled = 0;
			int module1ErrorCalled = 0;
			Exception module1Error = null;
			int module1FinishedCalled = 0;

			int module2BeforeCalled = 0;
			int module2AfterCalled = 0;
			int module2ErrorCalled = 0;
			Exception module2Error = null;
			int module2FinishedCalled = 0;

			int module3BeforeCalled = 0;
			int module3AfterCalled = 0;
			int module3ErrorCalled = 0;
			Exception module3Error = null;
			int module3FinishedCalled = 0;

			bool doWorkCalled = false;

			var module1 = new TestModule
			{
				BeforeCallback = delegate { module1BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module1AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module1ErrorCalled = ++callOrder;
					module1Error = ex;
				},
				FinishedCallback = delegate { module1FinishedCalled = ++callOrder; },
			};

			var module2 = new TestModule
			{
				BeforeCallback = delegate
				{
					module2BeforeCalled = ++callOrder;
				},
				AfterCallback = delegate
				{
					module2AfterCalled = ++callOrder;

				},
				ErrorCallback = delegate(Exception ex)
				{
					module2ErrorCalled = ++callOrder;
					module2Error = ex;
				},
				FinishedCallback = delegate
				                   	{
				                   		module2FinishedCalled = ++callOrder;
										errorExceptionThrown = new ApplicationException("thrown by module");
										throw errorExceptionThrown;
				                   	},
			};

			var module3 = new TestModule
			{
				BeforeCallback = delegate { module3BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module3AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module3ErrorCalled = ++callOrder;
					module3Error = ex;
				},
				FinishedCallback = delegate { module3FinishedCalled = ++callOrder; },
			};

			var action = WorkerAction.Define()
				.DoWork(delegate
				        	{
				        		doWorkCalled = true;
				        	})
				.AddModule(module1)
				.AddModule(module2)
				.AddModule(module3)
				.Create();

			action.Run();

			Assert.AreEqual(1, module1BeforeCalled);
			Assert.AreEqual(2, module2BeforeCalled);
			Assert.AreEqual(3, module3BeforeCalled);
			Assert.AreEqual(4, module3AfterCalled);
			Assert.AreEqual(5, module2AfterCalled);
			Assert.AreEqual(6, module1AfterCalled);
			Assert.AreEqual(0, module3ErrorCalled);
			Assert.IsNull(module3Error);
			Assert.AreEqual(0, module2ErrorCalled);
			Assert.IsNull(module2Error);
			Assert.AreEqual(0, module1ErrorCalled);
			Assert.IsNull(module1Error);
			Assert.AreEqual(7, module3FinishedCalled);
			Assert.AreEqual(8, module2FinishedCalled);
			Assert.AreEqual(9, module1FinishedCalled);
			Assert.IsTrue(doWorkCalled);
		}

		[Test]
		public void Calls_module_functions_when_exception_thrown()
		{
			int callOrder = 0;
			int module1BeforeCalled = 0;
			int module1AfterCalled = 0;
			int module1ErrorCalled = 0;
			Exception module1Error = null;
			int module1FinishedCalled = 0;

			var module1 = new TestModule
			{
				BeforeCallback = delegate { module1BeforeCalled = ++callOrder; },
				AfterCallback = delegate { module1AfterCalled = ++callOrder; },
				ErrorCallback = delegate(Exception ex)
				{
					module1ErrorCalled = ++callOrder;
					module1Error = ex;
				},
				FinishedCallback = delegate { module1FinishedCalled = ++callOrder; },
			};

			var action = WorkerAction.Define()
				.DoWork(delegate { throw new ApplicationException("test exception"); })
				.AddModule(module1)
				.Create();

			action.Run();

			Assert.AreEqual(1, module1BeforeCalled);
			Assert.AreEqual(0, module1AfterCalled);
			Assert.AreEqual(2, module1ErrorCalled);
			Assert.IsNotNull(module1Error);
			Assert.AreEqual(3, module1FinishedCalled);
		}

		private class TestSynchronizationContext : SynchronizationContext
		{
			public bool IsInSend { get; private set; }

			public override void Post(SendOrPostCallback d, object state)
			{
				Assert.Fail("Should not call Post method");
			}

			public override void Send(SendOrPostCallback d, object state)
			{
				Assert.IsFalse(IsInSend);
				IsInSend = true;
				try
				{
					d(state);
				}
				finally
				{
					IsInSend = false;
				}
			}
		}

		private class TestModule : WorkerActionModule
		{
			public Action BeforeCallback;
			public Action AfterCallback;
			public Action<Exception> ErrorCallback;
			public Action FinishedCallback;

			public override void Before()
			{
				if (BeforeCallback != null)
				{
					BeforeCallback();
				}
			}

			public override void After()
			{
				if (AfterCallback != null)
				{
					AfterCallback();
				}
			}

			public override void Error(Exception ex)
			{
				if (ErrorCallback != null)
				{
					ErrorCallback(ex);
				}
			}

			public override void Finished()
			{
				if (FinishedCallback != null)
				{
					FinishedCallback();
				}
			}
		}
	}
}