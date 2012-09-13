using System;
using Castle.Windsor;
using Moq;
using NHibernate;
using NUnit.Framework;
using Quokka.Castle;
using Quokka.NH.Implementations;
using Quokka.NH.Tests.Support;
using Quokka.ServiceLocation;
using Quokka.UI.Tasks;

// ReSharper disable InconsistentNaming

namespace Quokka.NH.Tests
{
	[TestFixture]
	public class UITaskFindCompatibleSessionTests
	{
		public FakeViewDeck ViewDeck;
		public Mock<ISession> SessionMock;
		public IWindsorContainer Container;
		public bool DisposeCalled;

		[SetUp]
		public void SetUp()
		{
			ViewDeck = new FakeViewDeck();
			SessionMock = new Mock<ISession>();
			Container = new WindsorContainer();
			var serviceContainer = new WindsorServiceContainer(Container, () => new WindsorContainer());
			ServiceLocator.SetLocatorProvider(() => serviceContainer.Locator);
			DisposeCalled = false;
			SessionMock.Setup(s => s.Dispose()).Callback(() => DisposeCalled = true);
		}

		[TearDown]
		public void TearDown()
		{
			Container.Dispose();
			ServiceLocator.SetLocatorProvider(null);
		}

		[Test]
		public void No_session_associated_with_task()
		{
			var task = new TaskWithNoSession();
			task.Start(ViewDeck);

			Assert.IsNull(task.FindCompatibleSession("alias", () => SessionMock.Object));
		}

		[Test]
		public void Session_associated_with_presenter()
		{
			var task = new TaskWithPresenterWithSession();
			task.Start(ViewDeck);

			bool callback = false;

			var session = task.FindCompatibleSession("alias", () => {
				callback = true;
				return SessionMock.Object;
			});

			Assert.IsTrue(callback);
			Assert.AreSame(session, SessionMock.Object);

			// move to the next node
			Assert.IsFalse(DisposeCalled);
			Assert.IsInstanceOf<PresenterWithSession>(task.CurrentNode.Presenter);
			((PresenterWithSession) task.CurrentNode.Presenter).Next.Navigate();
			Assert.IsTrue(DisposeCalled);
		}

		[Test]
		public void Session_associated_with_task()
		{
			var task = new TaskWithSession();
			task.Start(ViewDeck);

			Assert.IsInstanceOf<PresenterWithoutSession>(task.CurrentNode.Presenter);


			bool callback = false;

			// even though the presenter has no session, we still get a session
			// because the task has a session
			var session1 = task.FindCompatibleSession("alias", () =>
			{
				callback = true;
				return SessionMock.Object;
			});

			Assert.IsTrue(callback);
			Assert.AreSame(session1, SessionMock.Object);

			var presenter = (PresenterWithoutSession) task.CurrentNode.Presenter;
			presenter.Next.Navigate();

			// Session should not be disposed when we transition to the next node
			// as it should now persist for the duration of the task.
			Assert.IsFalse(DisposeCalled);

			Assert.IsInstanceOf<PresenterWithSession>(task.CurrentNode.Presenter);

			var session2 = task.FindCompatibleSession("alias", () => {
				throw new AssertionException("Should not call this, a session already exists");
			});

			Assert.AreSame(session2, session1);

			// This should end the task. Ensure that the session is disposed
			// once the task is finished.
			Assert.IsFalse(DisposeCalled);
			((PresenterWithSession)task.CurrentNode.Presenter).Next.Navigate();
			Assert.IsTrue(task.IsComplete);
			Assert.IsTrue(DisposeCalled);
		}

		public class TaskWithNoSession : UITask
		{
			protected override void CreateNodes()
			{
				CreateNode()
					.SetView<View>()
					.SetPresenter<PresenterWithoutSession>();
			}
		}

		public class TaskWithPresenterWithSession : UITask
		{
			protected override void CreateNodes()
			{
				var node1 = CreateNode();
				var node2 = CreateNode();

				node1
					.SetView<View>()
					.SetPresenter<PresenterWithSession>()
					.NavigateTo(p => p.Next, node2);

				node2.SetView<View>()
					.SetPresenter<PresenterWithoutSession>()
					.NavigateTo(p => p.Next, null);
			}
		}

		[NHSession]
		public class TaskWithSession : UITask
		{
			protected override void CreateNodes()
			{
				var node1 = CreateNode();
				var node2 = CreateNode();

				node1.SetView<View>()
					.SetPresenter<PresenterWithoutSession>()
					.NavigateTo(p => p.Next, node2);

				node2.SetView<View>()
					.SetPresenter<PresenterWithSession>()
					.NavigateTo(p => p.Next, null);
			}
		}

		public interface IView {}

		public class View  {}

		public class PresenterWithoutSession : Presenter<View>
		{
			public INavigateCommand Next { get; set; }
		}

		[NHSession]
		public class PresenterWithSession : Presenter<View>
		{
			public INavigateCommand Next { get; set; }
		}


	}
}
