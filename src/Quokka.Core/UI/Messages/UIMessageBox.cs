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

using Quokka.UI.Messages.Internal;
using Quokka.UI.Tasks;

namespace Quokka.UI.Messages
{
	/// <summary>
	/// 	Used for displaying modal message boxes.
	/// </summary>
	public class UIMessageBox
	{
		public IViewDeck ViewDeck { get; set; }

		public virtual UIAnswer Show(UIMessage message)
		{
			using (var modalWindow = ViewDeck.CreateModalWindow())
			{
				using (var task = new MessageBoxTask(message))
				{
					task.TaskComplete += (o, e) => modalWindow.Dispose();
					task.Start(modalWindow.ViewDeck);
					modalWindow.ShowModal(true);
				}
			}

			if (message.SelectedAnswer != null && message.SelectedAnswer.Callback != null)
			{
				message.SelectedAnswer.Callback();
			}
			return message.SelectedAnswer;
		}
	}
}