using System;
using System.ComponentModel;
using System.Windows.Forms;
using Quokka.UI.Commands;
using Quokka.UI.Tasks;
using Quokka.WinForms.Commands;

namespace Quokka.WinForms
{
	[ToolboxItem(false)]
	public partial class ErrorReportView : UserControl, IErrorReportView
	{
		public ErrorReportView()
		{
			InitializeComponent();
			Load += ViewLoad;
			AbortCommand = new UICommand(abortButton);
			CancelCommand = new UICommand(cancelButton);
			RetryCommand = new UICommand(retryButton);

			AbortCommand.PropertyChanged += (o, e) => UpdateVisibility(abortButton, AbortCommand);
			CancelCommand.PropertyChanged += (o, e) => UpdateVisibility(cancelButton, CancelCommand);
			RetryCommand.PropertyChanged += (o, e) => UpdateVisibility(retryButton, RetryCommand);
		}

		private void ViewLoad(object sender, EventArgs e)
		{
			if (ErrorReport == null)
			{
				errorDetailTextBox.Text = "No error information available.";
			}
			else
			{
				errorDetailTextBox.Text = ErrorReport.ToString();
			}

			errorDetailTextBox.Select(0, 0);

			buttonPanel.Visible = AbortCommand.Enabled || CancelCommand.Enabled || RetryCommand.Enabled;
		}

		private void UpdateVisibility(Control control, IUICommand command)
		{
			control.Visible = command.Enabled;
			buttonPanel.Visible = AbortCommand.Enabled || CancelCommand.Enabled || RetryCommand.Enabled;
		}


		public IUICommand AbortCommand { get; private set; }

		public IUICommand CancelCommand { get; private set; }

		public IUICommand RetryCommand { get; private set; }

		public ErrorReport ErrorReport { get; set; }
	}
}
