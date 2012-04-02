using NUnit.Framework;
using Quokka.Castle;
using Quokka.ServiceLocation;
using Quokka.UI.Fakes;
using Quokka.UI.TestApp1;

// ReSharper disable InconsistentNaming

namespace Quokka.UI
{
	[TestFixture]
	public class UITaskTests
	{
		private FakeViewDeck _viewDeck;

		[SetUp]
		public void SetUp()
		{
			_viewDeck = new FakeViewDeck();
			var serviceContainer = ServiceContainerFactory.CreateContainer();
			var locator = serviceContainer.Locator;
			ServiceLocator.SetLocatorProvider(() => locator);
		}

		[TearDown]
		public void TearDown()
		{
			_viewDeck = null;
			ServiceLocator.SetLocatorProvider(null);
		}

		[Test]
		public void Can_access_nodes_before_start()
		{
			var task = new TestTask1();
			Assert.IsNotNull(task.Nodes);
			Assert.AreEqual(3, task.Nodes.Count);
		}

		[Test]
		public void IsRunning_set_to_correct_values()
		{
			var task = new TestTask1();

			Assert.IsFalse(task.IsRunning);
			task.Start(_viewDeck);
			Assert.IsTrue(task.IsRunning);
		}

		[Test]
		public void NavigateCommands_are_created()
		{
			var task = new TestTask1();
			task.Start(_viewDeck);

			var presenter = task.CurrentNode.Presenter as Presenter1;
			Assert.IsNotNull(presenter);
			Assert.IsNotNull(presenter.Next);
			Assert.IsNotNull(presenter.Error);
			Assert.AreNotSame(presenter.Next, presenter.Error);
		}

		[Test]
		public void Can_navigate_to_next_node()
		{
			var task = new TestTask1();
			task.Start(_viewDeck);

			var view1 = task.CurrentNode.View as View1;
			Assert.IsNotNull(view1);
			view1.ClickButton1();

			var view2 = task.CurrentNode.View as View2;
			Assert.IsNotNull(view2);
			view2.ClickBackButton();

			var view3 = task.CurrentNode.View as View1;
			Assert.IsNotNull(view3);
		}

		[Test]
		public void Task_finishes_when_next_node_is_null()
		{
			var taskCompleteEventRaised = false;
			var task = new TestTask1();
			task.TaskComplete += delegate { taskCompleteEventRaised = true; };
			task.Start(_viewDeck);

			var view1 = task.CurrentNode.View as View1;
			Assert.IsNotNull(view1);
			view1.ClickButton1();

			var view2 = task.CurrentNode.View as View2;
			Assert.IsNotNull(view2);
			view2.ClickCloseButton();

			Assert.IsFalse(task.IsRunning);
			Assert.IsTrue(task.IsComplete);
			Assert.IsTrue(taskCompleteEventRaised);
		}

		[Test]
		public void Task_finishes_when_EndTask_called()
		{
			var taskCompleteEventRaised = false;
			var task = new TestTask1();
			task.TaskComplete += delegate { taskCompleteEventRaised = true; };
			task.Start(_viewDeck);

			var view1 = task.CurrentNode.View as View1;
			Assert.IsNotNull(view1);
			view1.ClickButton1();

			task.EndTask();

			Assert.IsFalse(task.IsRunning);
			Assert.IsTrue(task.IsComplete);
			Assert.IsTrue(taskCompleteEventRaised);
		}

		[Test]
		public void Container_injects_task_objects()
		{
			var task = new TestTask1();
			task.Start(_viewDeck);

			var presenter1 = task.CurrentNode.Presenter as Presenter1;
			Assert.IsNotNull(presenter1);
			Assert.AreSame(task, presenter1.UITask);
			Assert.AreSame(task, presenter1.TestTask1);
			Assert.AreSame(_viewDeck, presenter1.ViewDeck);
			Assert.AreSame(task.Nodes[0], presenter1.UINode);
		}

		[Test]
		public void Container_injects_different_navigate_command_objects()
		{
			var task = new TestTask1();
			task.Start(_viewDeck);

			var presenter1 = task.CurrentNode.Presenter as Presenter1;
			Assert.IsNotNull(presenter1);
			Assert.IsNotNull(presenter1.Next);
			Assert.IsNotNull(presenter1.Error);
			Assert.AreNotSame(presenter1.Next, presenter1.Error);
		}
	}
}