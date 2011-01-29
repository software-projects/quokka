using Dashboard.UI.TaskStates;
using Dashboard.UI.Views.Interfaces;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Presenters
{
	public class ShellPresenter : Presenter<IShellView>
	{
		public UserState UserState { get; set; }
		public INavigateCommand LogoutCommand { get; set; }
		public INavigateCommand DoSomethingCommand { get; set; }

		protected override void InitializePresenter()
		{
			View.Username = UserState.User.FullName;
			View.Logout += (sender, args) => LogoutCommand.Navigate();
			View.DoSomething += (sender, args) => DoSomethingCommand.Navigate();
		}
	}
}