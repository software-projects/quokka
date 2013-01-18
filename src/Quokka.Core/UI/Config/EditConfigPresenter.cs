using System;
using Quokka.Events;
using Quokka.UI.Messages;
using Quokka.UI.Tasks;

namespace Quokka.UI.Config
{
	public class EditConfigPresenter : Presenter<IEditConfigView>
	{
		public INavigateCommand SaveCommand { get; set; }
		public INavigateCommand CancelCommand { get; set; }
		public INavigateCommand ErrorCommand { get; set; }
		public ConfigTaskState TaskState { get; set; }
		public IEventBroker EventBroker { get; set; }
		public UIMessageBox MessageBox { get; set; }
		public ErrorReport ErrorReport { get; set; }

		public override void InitializePresenter()
		{
			View.SaveCommand.Execute += HandleSaveCommand;
			View.CancelCommand.Execute += (sender, args) => CancelCommand.Navigate();
			View.Parameter = TaskState.ConfigParameter;
		}

		private void HandleSaveCommand(object sender, EventArgs e)
		{
			try
			{
				var errorMessage = TaskState.ConfigParameter.ValidateText(View.Value);
				if (errorMessage != null)
				{
					MessageBox.Show(new UIMessage {
						MainInstruction = "Invalid Value",
						Content = errorMessage,
						MessageType = UIMessageType.Forbidden,
					});
					return;
				}

				TaskState.ConfigParameter.SetValueText(View.Value);

				SaveCommand.Navigate();
			}
			catch (Exception ex)
			{
				ErrorReport.ReportError("Saving configuration", ex);
				ErrorCommand.Navigate();
			}
		}
	}
}
