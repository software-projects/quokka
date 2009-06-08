using Quokka.Diagnostics;
using Quokka.UI;
using Quokka.Uip;
using Quokka.WinForms.Workspaces;

namespace Example3.Tasks.Simple
{
	public class SimplePresenter
	{
		private readonly IWorkspace _workspace;
		private readonly SimpleState _state;
		private readonly IUipNavigator _navigator;

		public SimplePresenter(IWorkspace workspace, IWorkspaceInfo workspaceInfo, SimpleState state, IUipNavigator navigator)
		{
			Verify.ArgumentNotNull(workspace, "workspace", out _workspace);
			Verify.ArgumentNotNull(workspaceInfo, "workspaceInfo");
			Verify.ArgumentNotNull(state, "state", out _state);
			Verify.ArgumentNotNull(navigator, "navigator", out _navigator);
			workspaceInfo.Text = "Task " + _state.TaskNumber;
			workspaceInfo.CanClose = state.TaskNumber > 1;
			workspaceInfo.Activate();
		}

		public void CreateNewTask()
		{
			UipTask newTask = new SimpleTask();
			_workspace.StartTask(newTask);
		}

		public void Close()
		{
			_navigator.Navigate("end");
		}
	}
}