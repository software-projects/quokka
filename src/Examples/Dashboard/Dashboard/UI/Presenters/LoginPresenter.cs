using System;
using Dashboard.UI.TaskStates;
using Dashboard.UI.Views.Interfaces;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Presenters
{
	public class LoginPresenter : Presenter<ILoginView>
	{
		public INavigateCommand NextCommand { get; set; }
		public LoginState LoginState { get; set; }
		public UserState UserState { get; set; }


		protected override void InitializePresenter()
		{
			UserState.User = null;

			if (!String.IsNullOrEmpty(LoginState.UserName))
			{
				View.Username = LoginState.UserName;
				View.Password = string.Empty;
				View.ErrorMessage = "Invalid username or password";
			}

			View.LoginCommand.Execute += delegate { AttemptLogin(); };
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

			LoginState.UserName = username;
			LoginState.Password = password;
			NextCommand.Navigate();
		}
	}
}