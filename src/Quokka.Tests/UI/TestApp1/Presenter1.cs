using System;
using NUnit.Framework;
using Quokka.UI.Tasks;

namespace Quokka.UI.TestApp1
{
	public class TestTask1 : UITask
	{
		protected override void CreateNodes()
		{
			var node1 = CreateNode("Node1");
			var node2 = CreateNode("Node2");
			var errorNode = CreateNode("ErrorNode");

			node1
				.SetView<View1>()
				.SetPresenter<Presenter1>()
				.NavigateTo(p => p.Next, node2)
				.NavigateTo(p => p.Error, errorNode);

			node2
				.SetView<View2>()
				.SetPresenter<Presenter2>()
				.NavigateTo(p => p.Back, node1)
				.NavigateTo(p => p.Error, errorNode)
				.NavigateTo(p => p.Close, null);

			errorNode
				.SetView<ErrorView>()
				.NavigateTo(v => v.Retry, node1);
		}
	}

	public interface IView1
	{
		event EventHandler Button1Click;
	}

	public class View1 : IView1
	{
		public event EventHandler Button1Click;

		public void ClickButton1()
		{
			if (Button1Click != null)
			{
				Button1Click(this, EventArgs.Empty);
			}
		}
	}

	public class Presenter1 : Presenter<IView1>
	{
		public INavigateCommand Next { get; set; }
		public INavigateCommand Error { get; set; }

		// for testing that these objects are 'injected' by the container
		public UITask UITask { get; set; }
		public IViewDeck ViewDeck { get; set; }
		public TestTask1 TestTask1 { get; set; }
		public UINode UINode { get; set; }

		public override void InitializePresenter()
		{
			Assert.IsTrue(UITaskContext.HasTask);
			View.Button1Click += (sender, args) => Next.Navigate();
		}
	}

	public interface IView2
	{
		event EventHandler BackButtonClick;
		event EventHandler CloseButtonClick;
	}

	public class View2 : IView2
	{
		public event EventHandler BackButtonClick;
		public event EventHandler CloseButtonClick;

		public void ClickBackButton()
		{
			if (BackButtonClick != null)
			{
				BackButtonClick(this, EventArgs.Empty);
			}
		}

		public void ClickCloseButton()
		{
			if (CloseButtonClick != null)
			{
				CloseButtonClick(this, EventArgs.Empty);
			}
		}
	}

	public class ErrorView
	{
		public INavigateCommand Retry { get; set; }
		
	}

	public class Presenter2 : Presenter<IView2>
	{
		public INavigateCommand Back { get; set; }
		public INavigateCommand Error { get; set; }
		public INavigateCommand Close { get; set; }

		public override void InitializePresenter()
		{
			View.BackButtonClick += delegate { Back.Navigate(); };
			View.CloseButtonClick += delegate { Close.Navigate(); };
		}
	}
}