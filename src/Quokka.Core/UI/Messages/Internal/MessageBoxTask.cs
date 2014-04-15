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

using Quokka.Diagnostics;
using Quokka.UI.Tasks;

namespace Quokka.UI.Messages.Internal
{
	internal class MessageBoxTask : UITask
	{
		public readonly UIMessage Message;

		public MessageBoxTask(UIMessage message)
		{
			Message = Verify.ArgumentNotNull(message, "message");
		}

		protected override void CreateNodes()
		{
			var node = CreateNode();

			node.SetPresenter<MessageBoxPresenter>()
				.NavigateTo(p => p.NextCommand, null);
		}

		protected override void CreateState()
		{
			RegisterInstance(Message);
		}
	}
}