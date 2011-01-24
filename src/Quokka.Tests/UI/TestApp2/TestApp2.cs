using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Quokka.Castle;
using Quokka.UI.Commands;
using Quokka.UI.Fakes;
using Quokka.UI.Tasks;

// ReSharper disable InconsistentNaming
namespace Quokka.UI.TestApp2
{
	[TestFixture]
	public class TestTask2Tests
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
		public void Can_navigate_to_nested_task()
		{
			var task = new TestTask2();
			task.Start(_viewDeck);

			var view1 = task.CurrentNode.View as View1;
			Assert.IsNotNull(view1, "Missing View1");
			view1.MoveNext();

			var view3 = task.CurrentNode.NestedTask.CurrentNode.View as View3;
			Assert.IsNotNull(view3, "Missing View3");
			view3.MoveNext();

			var view4 = task.CurrentNode.NestedTask.CurrentNode.View as View4;
			Assert.IsNotNull(view4, "Missing View4");
			view4.MoveNext();

			var view5 = task.CurrentNode.View as View5;
			Assert.IsNotNull(view5, "Missing View5");
		}
		
	}

	public class TestTask2 : UITask
	{
		protected override void CreateNodes()
		{
			var node1 = CreateNode("Node1");
			var node2 = CreateNode("Node2");
			var node5 = CreateNode("Node5");

			node1
				.SetView<View1>()
				.SetPresenter<Presenter1>()
				.NavigateTo(p => p.Next, node2);

			node2
				.SetNestedTask<NestedTask2>()
				.NavigateTo(t => t.Failed, node5)
				.NavigateTo(node5);

			node5
				.SetView<View5>()
				.NavigateTo(v => v.Close, null);
		}
	}

	public class NestedTask2 : UITask
	{
		public INavigateCommand Failed { get; set; }

		protected override void CreateNodes()
		{
			var node3 = CreateNode("Node3");
			var node4 = CreateNode("Node4");

			node3
				.SetView<View3>()
				.NavigateTo(v => v.Next, node4);

			node4
				.SetView<View4>()
				.NavigateTo(v => v.Next, null)
				.NavigateTo(v => v.Back, node3);
		}
	}

	public interface IView1
	{
		IUICommand NextCommand { get; }
	}

	public class View1 : IView1
	{
		public IUICommand NextCommand { get; private set; }

		public View1()
		{
			NextCommand = new FakeCommand();
		}

		public void MoveNext()
		{
			NextCommand.PerformExecute();
		}
	}

	public class Presenter1 : Presenter<IView1>
	{
		public INavigateCommand Next { get; set; }

		protected override void OnViewCreated()
		{
			View.NextCommand.Execute += (sender, args) => Next.Navigate();
		}
	}

	public class View3
	{
		public INavigateCommand Next { get; set; }

		public void MoveNext()
		{
			Next.Navigate();
		}
	}

	public class View4
	{
		public INavigateCommand Next { get; set; }
		public INavigateCommand Back { get; set; }

		public void MoveNext()
		{
			Next.Navigate();
		}

		public void MoveBack()
		{
			Back.Navigate();
		}
	}

	public class View5
	{
		public INavigateCommand Close { get; set; }

		public void CloseWindow()
		{
			Close.Navigate();
		}
	}
}