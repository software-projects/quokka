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

using Quokka.UI.Tasks;

namespace Quokka.UI.Config
{
	public class ConfigTask : UITask
	{
		protected override void CreateNodes()
		{
			var listNode = CreateNode();
			var editNode = CreateNode();
			var errorNode = CreateNode();

			listNode
				.SetPresenter<ListConfigPresenter>()
				.NavigateTo(p=> p.EditCommand, editNode)
				.NavigateTo(p => p.ErrorCommand, errorNode);

			editNode
				.SetPresenter<EditConfigPresenter>()
				.NavigateTo(p => p.SaveCommand, listNode)
				.NavigateTo(p => p.CancelCommand, listNode)
				.NavigateTo(p => p.ErrorCommand, errorNode);

			
			errorNode
				.SetPresenter<ErrorReportPresenter>()
				.NavigateTo(p => p.RetryCommand, listNode);
		}

		protected override void CreateState()
		{
			RegisterInstance(new ConfigTaskState());
		}
	}
}