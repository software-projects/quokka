using System;
using NUnit.Framework;
using Quokka.Events.Internal;

namespace Quokka.Events
{
	[TestFixture]
	public class EventBrokerTests
	{
		[Test]
		public void Creates_event_instance()
		{
			IEventBroker eventBroker = new EventBroker();
			TestEvent1 e = eventBroker.GetEvent<TestEvent1>();
			Assert.IsNotNull(e);
		}

		[Test]
		public void Creates_only_one_instance_for_each_type()
		{
			IEventBroker eventBroker = new EventBroker();
			TestEvent1 e1 = eventBroker.GetEvent<TestEvent1>();
			TestEvent2 e2 = eventBroker.GetEvent<TestEvent2>();
			TestEvent1 e3 = eventBroker.GetEvent<TestEvent1>();
			Assert.AreSame(e1, e3);
			Assert.AreNotSame(e1, e2);
		}

		[Test]
		public void Events_created_by_event_broker_have_EventBroker_property_set()
		{
			IEventBroker eventBroker = new EventBroker();
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

			IEventBroker eventBroker = new EventBroker();
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
			IEventBroker eventBroker = new EventBroker();
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

		private class TestEvent1 : Event<string>
		{
		}

		private class TestEvent2 : Event<int> {}

		private class TestEvent3 : Event {}

		private class TestEvent4 : Event {}
	}
}