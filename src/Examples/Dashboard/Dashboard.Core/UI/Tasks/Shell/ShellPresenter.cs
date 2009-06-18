using Dashboard.UI.Forms.Interfaces;
using Quokka.Diagnostics;

namespace Dashboard.UI.Tasks.Shell
{
	public class ShellPresenter
	{
		private readonly ShellState _state;
		private readonly INavigator _navigator;
		private IShellForm _view;

		public interface INavigator
		{
			void Logout();
		}

		public ShellPresenter(ShellState state, INavigator navigator)
		{
			Verify.ArgumentNotNull(state, "state", out _state);
			Verify.ArgumentNotNull(navigator, "navigator", out _navigator);
		}

		public void SetView(IShellForm view)
		{
			Verify.ArgumentNotNull(view, "view", out _view);
			view.Username = _state.User.FullName;
			view.Logout += delegate { Logout(); };
		}

		private void Logout()
		{
			_state.UserName = null;
			_state.Password = null;
			_state.User = null;
			_navigator.Logout();
		}
	}
}