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

using System.Threading;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Quokka.Threading
{
	[TestFixture]
	public class WorkerTests
	{
		private Worker _worker;

		[SetUp]
		public void SetUp()
		{
			_worker = new Worker();
		}

		[TearDown]
		public void TearDown()
		{
			_worker = null;
		}

		[Test]
		public void Run_one_action_without_SynchronizationContext()
		{
			var doWorkCalled = false;
			var whenCompleteCalled = false;

			WorkerAction.Define()
				.DoWork(() => doWorkCalled = true)
				.WhenComplete((o) => whenCompleteCalled = true)
				.Run(_worker);

			Assert.IsTrue(doWorkCalled);
			Assert.IsTrue(whenCompleteCalled);
		}

		[Test]
		public void Starting_and_Stopped_event_called_without_SynchronizationContext()
		{
			var startingRaised = 0;
			var stoppedRaised = 0;
			var doWorkCalled = 0;
			var whenCompleteCalled = 0;
			var counter = 0;

			_worker.Starting += delegate { startingRaised = ++counter; };
			_worker.Stopped += delegate { stoppedRaised = ++counter; };

			WorkerAction.Define()
				.DoWork(delegate { doWorkCalled = ++counter; })
				.WhenComplete(delegate { whenCompleteCalled = ++counter; })
				.Run(_worker);

			Assert.AreEqual(1, startingRaised);
			Assert.AreEqual(2, doWorkCalled);
			Assert.AreEqual(3, whenCompleteCalled);
			Assert.AreEqual(4, stoppedRaised);
		}

		[Test]
		public void Starting_and_Stopped_event_called_with_two_actions_and_with_SynchronizationContext()
		{
			var sc = new TestSynchronizationContext();
			_worker.SynchronizationContext = sc;

			var counter = 0;
			var startingRaised = 0;
			var stoppedRaised = 0;

			var doWork1Called = 0;
			var whenComplete1Called = 0;
			var doWorkWaitHandle1 = new ManualResetEvent(false);

			var doWork2Called = 0;
			var whenComplete2Called = 0;
			var doWorkWaitHandle2 = new ManualResetEvent(false);

			var completeWaitHandle = new ManualResetEvent(false);
			var stoppedRaisedWaitHandle = new ManualResetEvent(false);

			_worker.Starting += delegate
			                    	{
			                    		Assert.IsTrue(sc.IsInSend);
			                    		startingRaised = Interlocked.Increment(ref counter);
			                    	};
			_worker.Stopped += delegate
			                   	{
									Assert.IsTrue(sc.IsInSend);
			                   		stoppedRaised = Interlocked.Increment(ref counter);
			                   		stoppedRaisedWaitHandle.Set();
			                   		completeWaitHandle.WaitOne();
			                   	};

			// start one worker
			WorkerAction.Define()
				.DoWork(delegate
				        	{
				        		doWork1Called = Interlocked.Increment(ref counter);
				        		doWorkWaitHandle1.Set();
				        		completeWaitHandle.WaitOne();
				        	})
				.WhenComplete(delegate
				              	{
									Assert.IsTrue(sc.IsInSend);
				              		whenComplete1Called = ++counter;
				              	})
				.Run(_worker);

			// wait for the worker to start
			doWorkWaitHandle1.WaitOne();

			Assert.AreEqual(1, startingRaised);
			Assert.AreEqual(2, doWork1Called);
			Assert.AreEqual(0, stoppedRaised);

			// start another work action while the first one is still running
			WorkerAction.Define()
				.DoWork(delegate
				{
					doWork2Called = Interlocked.Increment(ref counter);
					doWorkWaitHandle2.Set();
				})
				.WhenComplete(delegate
				              	{
									Assert.IsTrue(sc.IsInSend);
				              		whenComplete2Called = ++counter;
				              	})
				.Run(_worker);

			doWorkWaitHandle2.WaitOne();

			Assert.AreEqual(1, startingRaised); // did not get called again
			Assert.AreEqual(2, doWork1Called);
			Assert.AreEqual(3, doWork2Called);
			Assert.AreEqual(0, stoppedRaised);

			completeWaitHandle.Set();
			stoppedRaisedWaitHandle.WaitOne();

			Assert.AreEqual(1, startingRaised); // did not get called again
			Assert.AreEqual(2, doWork1Called);
			Assert.AreEqual(3, doWork2Called);
			// one should be 4 and one should be 5, but they could be either order
			Assert.AreEqual(9, whenComplete1Called + whenComplete2Called);
			Assert.AreEqual(6, stoppedRaised);
		}

		private class TestSynchronizationContext : SynchronizationContext
		{
			private int sendCount = 0;

			public bool IsInSend
			{
				get { return sendCount > 0; }
			}

			public override void Post(SendOrPostCallback d, object state)
			{
				Assert.Fail("Should not call Post method");
			}

			public override void Send(SendOrPostCallback d, object state)
			{
				Interlocked.Increment(ref sendCount);
				try
				{
					d(state);
				}
				finally
				{
					Interlocked.Decrement(ref sendCount);
				}
			}
		}
	}
}