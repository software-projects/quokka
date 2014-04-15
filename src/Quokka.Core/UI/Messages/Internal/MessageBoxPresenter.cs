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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quokka.UI.Tasks;

namespace Quokka.UI.Messages.Internal
{
	internal class MessageBoxPresenter : Presenter<IUIMessageBoxView>
	{
		public INavigateCommand NextCommand { get; set; }
		public UIMessage Message { get; set; }

		public override void InitializePresenter()
		{
			View.Message = Message;
			View.Answered += AnsweredHandler;
		}

		private void AnsweredHandler(object sender, EventArgs e)
		{
			NextCommand.Navigate();
		}
	}
}
