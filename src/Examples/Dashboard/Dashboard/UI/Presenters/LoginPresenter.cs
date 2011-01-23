using System;
using Dashboard.UI.TaskStates;
using Dashboard.UI.Views.Interfaces;
using Quokka.Diagnostics;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Presenters
{
	public class LoginPresenter : Presenter<ILoginView>
	{
		private readonly LoginState _state;

		public INavigateCommand Next { get; set; }


		public LoginPresenter(LoginState state)
		{
			Verify.ArgumentNotNull(state, "state", out _state);
		}

		protected override void OnViewCreated()
		{
			if (!String.IsNullOrEmpty(_state.UserName))
			{
				View.Username = _state.UserName;
				View.Password = string.Empty;
				View.ErrorMessage = "Invalid username or password";
			}

			View.Login += delegate { AttemptLogin(); };			
		}


		private void AttemptLogin()
		{
			string username = (View.Username ?? string.Empty).Trim();
			string password = (View.Password ?? string.Empty).Trim();
			if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
			{
				View.ErrorMessage = "Please enter a username and password";
				return;
			}

			_state.UserName = username;
			_state.Password = password;
			Next.Navigate();
		}
	}
}