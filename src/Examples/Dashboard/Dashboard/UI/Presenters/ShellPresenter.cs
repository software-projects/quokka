using System;
using Dashboard.UI.TaskStates;
using Dashboard.UI.Views.Interfaces;
using Quokka.UI.Messages;
using Quokka.UI.Tasks;

namespace Dashboard.UI.Presenters
{
	public class ShellPresenter : Presenter<IShellView>
	{
		public UserState UserState { get; set; }
		public INavigateCommand LogoutCommand { get; set; }
		public INavigateCommand DoSomethingCommand { get; set; }
		public UIMessageBox MessageBox { get; set; }

		public override void InitializePresenter()
		{
			View.Username = UserState.User.FullName;
			View.Logout += (sender, args) => LogoutCommand.Navigate();
			View.DoSomething += DoSomething;
		}

		private void DoSomething(object sender, EventArgs e)
		{
			var message = new UIMessage
			              	{
			              		MainInstruction = "Do Something",
			              		Content = "This is going to do something in a modal window."
			              		          + Environment.NewLine
			              		          + Environment.NewLine
			              		          + "Do you want to do this?",
			              		PossibleAnswers =
			              			{
			              				new UIAnswer(UIAnswerType.Yes),
			              				new UIAnswer(UIAnswerType.No)
			              			}
			              	};

			var answer = MessageBox.Show(message);
			if (answer != null && answer.AnswerType == UIAnswerType.Yes)
			{
				DoSomethingCommand.Navigate();
			}
		}
	}
}