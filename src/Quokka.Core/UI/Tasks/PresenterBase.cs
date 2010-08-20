using System;
using System.Collections.Generic;
using System.Linq;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;
using Quokka.UI.Tasks;

namespace Quokka.UI.Tasks
{

	public interface ISampleView
	{
		bool ShowSomething { set; }
	}

	public interface IStandaloneView
	{
		NavigateCommand Ok { get; }
		NavigateCommand Cancel { get; }
	}

	public class SamplePresenter : Presenter<ISampleView>
	{
		public readonly NavigateCommand Next = new NavigateCommand();
		public readonly NavigateCommand Error = new NavigateCommand();

		public string Title { get; set; }

		SamplePresenter(ISampleView view)
		{
			View = Verify.ArgumentNotNull(view, "view");
		}
	}

	public class SampleTask : UITask
	{
		protected override void CreateNodes()
		{
			var startNode = CreateNode();
			var nextNode = CreateNode();
			var errorNode = CreateNode();

			startNode
				.SetView<ISampleView>()
				.With(v => v.ShowSomething = true)
				.SetPresenter<SamplePresenter>()
				.With(p => p.Title = "Some Title")
				.NavigateTo(p => p.Next, nextNode)
				.NavigateTo(p => p.Error, errorNode);

			nextNode
				.SetPresenter<SamplePresenter>()
				.NavigateTo(p => p.Next, startNode);

			errorNode
				.SetView<IStandaloneView>()
				.NavigateTo(v => v.Cancel, null)
				.NavigateTo(v => v.Ok, null);
		}
	}

	[PerRequest(typeof(ISampleView))]
	public class SampleView : ISampleView
	{
		public bool ShowSomething
		{
			set { throw new NotImplementedException(); }
		}
	}
}