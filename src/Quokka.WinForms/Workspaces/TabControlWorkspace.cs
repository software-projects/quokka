using System.Drawing;
using System.Windows.Forms;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;
using Quokka.UI;
using Quokka.Uip;

namespace Quokka.WinForms.Workspaces
{
	public class TabControlWorkspace : Workspace
	{
		private readonly TabControl _control;

		public TabControlWorkspace(TabControl control)
		{
			Verify.ArgumentNotNull(control, "control", out _control);
		}

		protected override IUipViewManager GetViewManager(UipTask task)
		{
			TabPage tabPage = new TabPage();
			WorkspaceInfo workspaceInfo = new WorkspaceInfo(this, tabPage);
			task.ServiceContainer.RegisterInstance<IWorkspaceInfo>(workspaceInfo);
			task.ServiceContainer.RegisterInstance<IWorkspace>(this);
			ViewManager viewManager = new ViewManager(tabPage);
			viewManager.AllTasksComplete += delegate { _control.TabPages.Remove(tabPage); };
			_control.TabPages.Add(tabPage);
			return viewManager;
		}

		private class WorkspaceInfo : IWorkspaceInfo
		{
			private readonly TabPage _tabPage;
			private readonly TabControlWorkspace _workspace;

			public WorkspaceInfo(TabControlWorkspace workspace, TabPage tabPage)
			{
				Verify.ArgumentNotNull(workspace, "workspace", out _workspace);
				Verify.ArgumentNotNull(tabPage, "tabPage", out _tabPage);
			}

			public string Text
			{
				get { return _tabPage.Text; }
				set { _tabPage.Text = value; }
			}

			public Image Image
			{
				get { return null; }
				set
				{
					/* do nothing */
				}
			}

			public bool CanClose
			{
				get { return false; }
				set
				{
					/* do nothing */
				}
			}

			public void Activate()
			{
				_workspace._control.SelectTab(_tabPage);
			}
		}
	}
}