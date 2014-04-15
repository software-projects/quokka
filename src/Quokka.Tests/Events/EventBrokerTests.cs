using System;
using NUnit.Framework;
using Quokka.Events.Internal;

namespace Quokka.Events
{
	[TestFixture]
	public class EventBrokerTests
	{
		private static int _testActionCount;
		private TestAction _testAction;

		[Test]
		public void Creates_event_instance()
		{
			IEventBroker eventBroker = new EventBrokerImpl();
			TestEvent1 e = eventBroker.GetEvent<TestEvent1>();
			Assert.IsNotNull(e);
		}

		[Test]
		public void Creates_only_one_instance_for_each_type()
		{
			IEventBroker eventBroker = new EventBrokerImpl();
			TestEvent1 e1 = eventBroker.GetEvent<TestEvent1>();
			TestEvent2 e2 = eventBroker.GetEvent<TestEvent2>();
			TestEvent1 e3 = eventBroker.GetEvent<TestEvent1>();
			Assert.AreSame(e1, e3);
			Assert.AreNotSame(e1, e2);
		}

		[Test]
		public void Events_created_by_event_broker_have_EventBroker_property_set()
		{
			IEventBroker eventBroker = new EventBrokerImpl();
			TestEvent1 e1 = eventBroker.GetEvent<TestEvent1>();
			Assert.AreSame(e1.EventBroker, eventBroker);

			TestEvent1 e2 = new TestEvent1();
			Assert.IsNull(e2.EventBroker);
		}

		[Test]
		public void Subscribe_and_publish_nongeneric_events()
		{
			int e3Count = 0;
			int e4Count = 0;

			IEventBroker eventBroker = new EventBrokerImpl();
			TestEvent3 e3 = eventBroker.GetEvent<TestEvent3>();
			TestEvent4 e4 = eventBroker.GetEvent<TestEvent4>();
			Assert.AreNotSame(e3, e4);
			e3.Subscribe(() => e3Count++);
			e4.Subscribe(() => e4Count++);
			e3.Publish();
			e3.Publish();
			e4.Publish();
			Assert.AreEqual(2, e3Count);
			Assert.AreEqual(1, e4Count);
		}

		[Test]
		public void Subscribe_and_publish_generic_events()
		{
			string e1Value = String.Empty;
			int e2Value = 0;
			IEventBroker eventBroker = new EventBrokerImpl();
			TestEvent1 e1 = eventBroker.GetEvent<TestEvent1>();
			TestEvent2 e2 = eventBroker.GetEvent<TestEvent2>();
			e1.Subscribe(i => e1Value = i);
			e2.Subscribe(i => e2Value = i);
			e1.Publish("Basingstoke");
			Assert.AreEqual("Basingstoke", e1Value);
			e1.Publish("XXX");
			Assert.AreEqual("XXX", e1Value);
			e2.Publish(42);
			Assert.AreEqual(42, e2Value);
		}

		[Test]
		public void Does_not_fire_weak_references()
		{
			_testActionCount = 0;
			IEventBroker eventBroker = new EventBrokerImpl();
			TestEvent3 e3 = eventBroker.GetEvent<TestEvent3>();

			// this is a bit tricky, the GC does not want to collect 
			// local vars in a method until it is finished, so we have
			// to have _testAction as a member variable, not a local variable.
			_testAction = new TestAction();

			e3.Subscribe(_testAction.DoSomething);

			// remove references to _testAction and then garbage collect.
			_testAction = null;
			GC.Collect();

			// publish again and it should not fire the action
			e3.Publish();

			// tests that the count did not get increased
			Assert.AreEqual(0, _testActionCount);
		}

		[Test]
		public void Does_not_fire_if_filter_returns_false()
		{
			IEventBroker eventBroker = new EventBrokerImpl();

			int result1 = 0;
			int result2 = 0;

			IEventSubscription<int> subscription1 = eventBroker.GetEvent<TestEvent2>()
				.Subscribe((i) => result1 = i)
				.SetFilter((i) => i % 2 != 0); // only works for odd numbers

			IEventSubscription<int> subscription2 = eventBroker.GetEvent<TestEvent2>()
				.Subscribe((i) => result2 = i)
				.SetFilter((i) => i%2 == 0); // only works for even numbers

			// raise an event for even number
			eventBroker.GetEvent<TestEvent2>().Publish(2);
			Assert.AreEqual(0, result1, "subscription1 should not receive an event with an even number as a payload");
			Assert.AreEqual(2, result2, "subscription2 should receive an event with an even number as a payload");

			// raise an event that will pass the filter
			result1 = 0;
			result2 = 0;
			eventBroker.GetEvent<TestEvent2>().Publish(333);
			Assert.AreEqual(333, result1, "subscription1 should receive an event with an odd number as a payload");
			Assert.AreEqual(0, result2, "subscription2 should not receive an event with an odd number as a payload");
		}

		private class TestEvent1 : Event<string>
		{
		}

		private class TestEvent2 : Event<int> {}

		private class TestEvent3 : Event {}

		private class TestEvent4 : Event {}

		private class TestAction
		{
			public void DoSomething()
			{
				_testActionCount++;
			}
		}
	}
}