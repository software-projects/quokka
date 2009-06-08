using System;
using System.Drawing;
using ComponentFactory.Krypton.Navigator;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;
using Quokka.UI;
using Quokka.Uip;
using Quokka.WinForms;
using Quokka.WinForms.Workspaces;

namespace Quokka.Krypton
{
	public class KryptonNavigatorWorkspace : Workspace
	{
		private readonly KryptonNavigator _navigator;

		public KryptonNavigatorWorkspace(KryptonNavigator navigator)
		{
			Verify.ArgumentNotNull(navigator, "navigator", out _navigator);
			_navigator.Selected += Navigator_Selected;
			_navigator.CloseAction += Navigator_CloseAction;
		}

		private static void Navigator_CloseAction(object sender, CloseActionEventArgs e)
		{
			KryptonPage page = e.Item;
			if (page == null)
				return;
			WorkspaceInfo workspaceInfo = page.Tag as WorkspaceInfo;
			if (workspaceInfo == null)
				return;
			workspaceInfo.Closed();
		}

		private static void Navigator_Selected(object sender, KryptonPageEventArgs e)
		{
			KryptonPage tabPage = e.Item;
			WorkspaceInfo workspaceInfo = tabPage.Tag as WorkspaceInfo;
			if (workspaceInfo != null)
			{
				workspaceInfo.Selected();
			}
		}

		protected override IUipViewManager GetViewManager(UipTask task)
		{
			KryptonPage tabPage = new KryptonPage();
			ViewManager viewManager = new ViewManager(tabPage);
			WorkspaceInfo workspaceInfo = new WorkspaceInfo(_navigator, tabPage, viewManager);
			tabPage.Tag = workspaceInfo;
			task.ServiceContainer.RegisterInstance<IWorkspaceInfo>(workspaceInfo);
			task.ServiceContainer.RegisterInstance<IWorkspace>(this);
			viewManager.AllTasksComplete += delegate
			                                	{
			                                		_navigator.Pages.Remove(tabPage);
			                                		viewManager.Clear();
			                                		tabPage.Dispose();
			                                	};
			_navigator.Pages.Add(tabPage);
			return viewManager;
		}

		private class WorkspaceInfo : IWorkspaceInfo
		{
			private readonly KryptonNavigator _navigator;
			private readonly KryptonPage _tabPage;
			private readonly ViewManager _viewManager;

			public WorkspaceInfo(KryptonNavigator navigator, KryptonPage tabPage, ViewManager viewManager)
			{
				Verify.ArgumentNotNull(navigator, "navigator", out _navigator);
				Verify.ArgumentNotNull(tabPage, "tabPage", out _tabPage);
				Verify.ArgumentNotNull(viewManager, "viewManager", out _viewManager);
			}

			public string Text
			{
				get { return _tabPage.Text; }
				set { _tabPage.Text = value; }
			}

			public Image Image
			{
				get { return _tabPage.ImageSmall; }
				set { _tabPage.ImageSmall = value; }
			}

			public bool CanClose { get; set; }

			public void Activate()
			{
				_navigator.SelectedPage = _tabPage;
			}

			public void Selected()
			{
				_navigator.Button.CloseButtonDisplay = CanClose ? ButtonDisplay.Logic : ButtonDisplay.ShowDisabled;
				_navigator.Button.CloseButtonAction = CloseButtonAction.RemovePage;
			}

			public void Closed()
			{
				_viewManager.Clear();
			}
		}
	}
}