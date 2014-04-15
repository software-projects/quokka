#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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
			UpdateView();
		}

		private void UpdateVisibility(Control control, IUICommand command)
		{
			control.Visible = command.Enabled;
			buttonPanel.Visible = AbortCommand.Enabled || CancelCommand.Enabled || RetryCommand.Enabled;
		}

		private void UpdateView()
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

		public IUICommand AbortCommand { get; private set; }

		public IUICommand CancelCommand { get; private set; }

		public IUICommand RetryCommand { get; private set; }

		private ErrorReport _errorReport;
		public ErrorReport ErrorReport
		{
			get { return _errorReport; }
			set
			{
				_errorReport = value;
				UpdateView();
			}
		}
	}
}
