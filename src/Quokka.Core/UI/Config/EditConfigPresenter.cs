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
			if (TaskState.ConfigParameter.IsReadOnly)
			{
				MessageBox.Show(new UIMessage {
					MainInstruction = "Read only",
					Content = "Cannot update this parameter, as it is read only",
					MessageType = UIMessageType.Forbidden,
				});
				return;
			}

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
