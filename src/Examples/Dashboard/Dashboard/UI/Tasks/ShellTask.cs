using Dashboard.UI.Presenters;
using Dashboard.UI.TaskStates;
using Dashboard.UI.Views;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Tasks
{
	public class ShellTask : UITask
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
				.NavigateTo(p => p.Next, loggingInNode);

			loggingInNode
				.SetPresenter<LoggingInPresenter>()
				.NavigateTo(p => p.Success, shellNode)
				.NavigateTo(p => p.Fail, loginNode);

			shellNode
				.SetView<ShellView>()
				.SetPresenter<ShellPresenter>()
				.NavigateTo(p => p.NavigateLogout, loginNode);
		}
	}
}