using Dashboard.UI.TaskStates;
using Dashboard.UI.Views.Interfaces;
using Quokka.Diagnostics;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Presenters
{
	public class ShellPresenter : Presenter<IShellView>
	{
		private readonly UserState _state;
		public INavigateCommand NavigateLogout { get; set; }

		public ShellPresenter(UserState state)
		{
			_state = Verify.ArgumentNotNull(state, "state");
		}

		protected override void OnViewCreated()
		{
			View.Username = _state.User.FullName;
			View.Logout += delegate { Logout(); };
		}

		private void Logout()
		{
			_state.User = null;
			NavigateLogout.Navigate();
		}
	}
}