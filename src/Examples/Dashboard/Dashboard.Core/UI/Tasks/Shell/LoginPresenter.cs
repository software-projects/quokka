using System;
using Dashboard.UI.Forms.Interfaces;
using Quokka.Diagnostics;

namespace Dashboard.UI.Tasks.Shell
{
	public class LoginPresenter
	{
		private readonly ShellState _state;
		private readonly INavigator _navigator;
		private ILoginForm _view;

		public interface INavigator
		{
			void Next();
		}

		public LoginPresenter(ShellState state, INavigator navigator)
		{
			Verify.ArgumentNotNull(state, "state", out _state);
			Verify.ArgumentNotNull(navigator, "navigator", out _navigator);
		}

		public void SetView(ILoginForm view)
		{
			Verify.ArgumentNotNull(view, "view", out _view);

			if (!String.IsNullOrEmpty(_state.UserName))
			{
				_view.Username = _state.UserName;
				_view.Password = string.Empty;
				_view.ErrorMessage = "Invalid username or password";
			}

			_view.Login += delegate { AttemptLogin(); };
		}

		private void AttemptLogin()
		{
			string username = (_view.Username ?? string.Empty).Trim();
			string password = (_view.Password ?? string.Empty).Trim();
			if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
			{
				_view.ErrorMessage = "Please enter a username and password";
				return;
			}

			_state.UserName = username;
			_state.Password = password;
			_navigator.Next();
		}
	}
}