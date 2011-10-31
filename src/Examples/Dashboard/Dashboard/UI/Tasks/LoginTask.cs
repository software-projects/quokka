using Dashboard.UI.Presenters;
using Dashboard.UI.TaskStates;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Tasks
{
	public class LoginTask : UITask
	{
		public readonly LoginState LoginState = new LoginState();
		public readonly UserState UserState = new UserState();

		protected override void CreateState()
		{
			RegisterInstance(LoginState);
			RegisterInstance(UserState);
		}

		protected override void CreateNodes()
		{
			var loginNode = CreateNode();
			var loggingInNode = CreateNode();
			var shellNode = CreateNode();

			loginNode
				.SetPresenter<LoginPresenter>()
				.NavigateTo(p => p.NextCommand, loggingInNode);

			loggingInNode
				.SetPresenter<LoggingInPresenter>()
				.NavigateTo(p => p.Success, shellNode)
				.NavigateTo(p => p.Fail, loginNode);

			shellNode
				.SetNestedTask<ShellTask>()
				.NavigateTo(loginNode);
		}
	}
}