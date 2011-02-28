using System;
using System.Threading;
using NUnit.Framework;

namespace Quokka.Sandbox
{
	[TestFixture]
	public class BackgroundActionTests
	{
		[Test]
		public void SimpleTest()
		{
			int result = 0;

			var action = BackgroundAction.Define()
				.DoWork(() => 2 + 2)
				.WhenComplete(n => result = n)
				.WithInitialDelayOf(100)
				.WithTransaction()
				.Create();

			action.Run();
			Assert.AreEqual(4, result);
		}

		[Test]
		public void ThrowsException()
		{
			bool whenCompleteCalled = false;
			Exception exception = null;

			var action = BackgroundAction.Define()
				.DoWork(delegate { throw new ApplicationException("Test exception"); })
				.WhenComplete(() => whenCompleteCalled = true)
				.WhenError(ex => exception = ex)
				.Create();

			action.Run();
			Assert.IsFalse(whenCompleteCalled);
			Assert.IsNotNull(exception);
			Assert.AreEqual("Test exception", exception.Message);
		}

		[Test]
		public void CallsCompleteViaSynchronizationContext()
		{
			var sc = new TestSynchronizationContext();
			bool doWorkWasRun = false;
			bool whenCompleteWasRun = false;

			var action = BackgroundAction.Define()
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
	}
}