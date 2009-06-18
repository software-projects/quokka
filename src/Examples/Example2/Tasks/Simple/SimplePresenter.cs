using Quokka.Diagnostics;
using Quokka.UI.Regions;
using Quokka.Uip;
using Quokka.WinForms.Regions;

namespace Example2.Tasks.Simple
{
	public class SimplePresenter
	{
		private readonly IRegion _region;
		private readonly SimpleState _state;
		private readonly IUipNavigator _navigator;

		public SimplePresenter(IRegion region, IRegionInfo regionInfo, SimpleState state, IUipNavigator navigator)
		{
			Verify.ArgumentNotNull(region, "region", out _region);
			Verify.ArgumentNotNull(state, "state", out _state);
			Verify.ArgumentNotNull(navigator, "navigator", out _navigator);

			
			regionInfo.Text = "Task " + _state.TaskNumber;
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