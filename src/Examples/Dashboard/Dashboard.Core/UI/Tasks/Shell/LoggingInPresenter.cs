using System.ComponentModel;
using System.Threading;
using Dashboard.Services.Interfaces;
using Dashboard.UI.Forms.Interfaces;
using Quokka.Diagnostics;

namespace Dashboard.UI.Tasks.Shell
{
	public class LoggingInPresenter
	{
		private readonly ShellState _state;
		private readonly INavigator _navigator;
		private readonly ILoginService _loginService;
		private readonly BackgroundWorker _backgroundWorker;

		public interface INavigator
		{
			void Success();
			void Fail();
		}

		public LoggingInPresenter(ShellState state, INavigator navigator, ILoginService loginService)
		{
			Verify.ArgumentNotNull(state, "state", out _state);
			Verify.ArgumentNotNull(navigator, "navigator", out _navigator);
			Verify.ArgumentNotNull(loginService, "loginService", out _loginService);
			_backgroundWorker = new BackgroundWorker();
			_backgroundWorker.DoWork += delegate { DoWork(); };
			_backgroundWorker.RunWorkerCompleted += delegate { WorkCompleted(); };
		}

		public void SetView(ILoggingInForm view)
		{
			view.Load += delegate { ViewLoaded(); };
		}

		private void ViewLoaded()
		{
			_backgroundWorker.RunWorkerAsync();
		}

		private void DoWork()
		{
			_state.User = _loginService.AttemptLogin(_state.UserName, _state.Password);
			_state.Password = null;
		}

		private void WorkCompleted()
		{
			if (_state.User != null && _state.User.Identity.IsAuthenticated)
			{
				Thread.CurrentPrincipal = _state.User;
				_navigator.Success();
			}
			else
			{
				Thread.CurrentPrincipal = null;
				_navigator.Fail();
			}

		}
	}
}