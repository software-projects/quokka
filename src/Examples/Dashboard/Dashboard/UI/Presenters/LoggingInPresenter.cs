using System.ComponentModel;
using System.Threading;
using Dashboard.Services.Interfaces;
using Dashboard.UI.TaskStates;
using Dashboard.UI.Views.Interfaces;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Presenters
{
	public class LoggingInPresenter : Presenter<ILoggingInView>
	{
		private readonly BackgroundWorker _backgroundWorker;

		public INavigateCommand Success { get; set; }
		public INavigateCommand Fail { get; set; }

		public LoginState LoginState { get; set; }
		public UserState UserState { get; set; }

		public ILoginService LoginService { get; set; }

		public LoggingInPresenter()
		{
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += delegate { DoWork(); };
			_backgroundWorker.RunWorkerCompleted += delegate { WorkCompleted(); };
		}

		protected override void InitializePresenter()
		{
			View.Load += delegate { ViewLoaded(); };
		}

		private void ViewLoaded()
		{
			_backgroundWorker.RunWorkerAsync();
		}

		private void DoWork()
		{
			UserState.User = LoginService.AttemptLogin(LoginState.UserName, LoginState.Password);

			// Clear out the supplied password, as it is not needed anymore.
			LoginState.Password = null;
		}

		private void WorkCompleted()
		{
			if (UserState.User != null && UserState.User.Identity.IsAuthenticated)
			{
				// Successful login means we do not need to retain the username anymore
				LoginState.UserName = null;

				Thread.CurrentPrincipal = UserState.User;
				Success.Navigate();
			}
			else
			{
				Thread.CurrentPrincipal = null;
				Fail.Navigate();
			}
		}
	}
}