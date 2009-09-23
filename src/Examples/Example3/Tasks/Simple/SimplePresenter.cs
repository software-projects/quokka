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
		private readonly IRegionManager _regionManager;

		public SimplePresenter(IRegion workspace, IRegionInfo regionInfo, SimpleState state, IUipNavigator navigator,
			IRegionManager regionManager)
		{
			Verify.ArgumentNotNull(workspace, "workspace", out _region);
			Verify.ArgumentNotNull(regionInfo, "regionInfo");
			Verify.ArgumentNotNull(state, "state", out _state);
			Verify.ArgumentNotNull(navigator, "navigator", out _navigator);
			Verify.ArgumentNotNull(regionManager, "regionManager", out _regionManager);
			regionInfo.Text = "Task " + _state.TaskNumber;
			regionInfo.CanClose = state.TaskNumber > 1;
		}

		public void CreateNewTask()
		{
			CreateNewTask(_region);
		}

		public void CreateNewModalTask()
		{
			CreateNewTask(_regionManager.Regions[RegionNames.ModalRegion]);
		}

		public void Close()
		{
			_navigator.Navigate("end");
		}

		private static void CreateNewTask(IRegion region)
		{
			UipTask newTask = new SimpleTask();
			region.Add(newTask);
			region.Activate(newTask);
		}
	}
}