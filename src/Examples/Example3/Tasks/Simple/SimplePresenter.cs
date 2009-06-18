using Quokka.Diagnostics;
using Quokka.UI.Regions;
using Quokka.Uip;
using Quokka.WinForms.Regions;

namespace Example3.Tasks.Simple
{
	public class SimplePresenter
	{
		private readonly IRegion _region;
		private readonly SimpleState _state;
		private readonly IUipNavigator _navigator;

		public SimplePresenter(IRegion workspace, IRegionInfo regionInfo, SimpleState state, IUipNavigator navigator)
		{
			Verify.ArgumentNotNull(workspace, "workspace", out _region);
			Verify.ArgumentNotNull(regionInfo, "regionInfo");
			Verify.ArgumentNotNull(state, "state", out _state);
			Verify.ArgumentNotNull(navigator, "navigator", out _navigator);
			regionInfo.Text = "Task " + _state.TaskNumber;
			regionInfo.CanClose = state.TaskNumber > 1;
		}

		public void CreateNewTask()
		{
			UipTask newTask = new SimpleTask();
			_region.Add(newTask);
			_region.Activate(newTask);
		}

		public void Close()
		{
			_navigator.Navigate("end");
		}
	}
}