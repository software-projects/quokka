using Dashboard.UI.Forms;
using Quokka.Uip;

namespace Dashboard.UI.Tasks.Shell
{
	public class ShellTask : UipTask<ShellState>
	{
		public readonly UipNode LoginNode = new UipNode();
		public readonly UipNode LoggingInNode = new UipNode();
		public readonly UipNode ShellNode = new UipNode();

		public ShellTask()
		{
			LoginNode
				.SetView<LoginForm>()
				.SetPresenter<LoginPresenter>()
				.NavigateTo("Next", LoggingInNode);

			LoggingInNode
				.SetView<LoggingInForm>()
				.SetPresenter<LoggingInPresenter>()
				.NavigateTo("Success", ShellNode)
				.NavigateTo("Fail", LoginNode);

			ShellNode
				.SetView<ShellForm>()
				.SetPresenter<ShellPresenter>()
				.NavigateTo("Logout", LoginNode);

		}
	}
}