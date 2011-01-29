using Dashboard.UI.Presenters;
using Dashboard.UI.TaskStates;
using Dashboard.UI.Views;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Tasks
{
	public class ShellTask : UITask
	{
		public UserState UserState { get; set; }

		protected override void CreateState()
		{
			RegisterInstance(UserState);
		}

		protected override void CreateNodes()
		{
			var shellNode = CreateNode();
			var confirmLogoutNode = CreateNode();
			var doSomethingNode = CreateNode();

			shellNode
				.SetView<ShellView>()
				.SetPresenter<ShellPresenter>()
				.NavigateTo(p => p.LogoutCommand, confirmLogoutNode)
				.NavigateTo(p => p.DoSomethingCommand, doSomethingNode);

			doSomethingNode
				.SetNestedTask<DoSomethingTask>()
				.NavigateTo(shellNode)
				.ShowModal();

			confirmLogoutNode
				.SetView<ConfirmLogoutView>()
				.NavigateTo(v => v.LogoutCommand, null)
				.NavigateTo(v => v.CancelCommand, shellNode)
				.ShowModal();
		}
	}
}