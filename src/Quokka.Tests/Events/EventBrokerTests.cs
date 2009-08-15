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
		public void Subscribe_and_publish_an_event()
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
	}
}