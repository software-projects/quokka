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
using System.Linq;
using Quokka.Config;
using Quokka.UI.Tasks;
using Quokka.WinForms;

namespace Quokka.UI.Config
{
	public class ListConfigPresenter : Presenter<IListConfigView>
	{
		public INavigateCommand ErrorCommand { get; set; }
		public INavigateCommand EditCommand { get; set; }
		public ErrorReport ErrorReport { get; set; }
		public ConfigTaskState TaskState { get; set; }

		private readonly VirtualDataSource<string, IConfigParameter> _dataSource =
			new VirtualDataSource<string, IConfigParameter>(p => p.Name);

		public override void InitializePresenter()
		{
			View.EditCommand.Execute += HandleEditCommand;
			View.RefreshCommand.Execute += HandleRefreshCommand;
			View.DataSource = _dataSource;
			Refresh();
		}

		private void Refresh()
		{
			try
			{
				// casting is necessary for .NET 3.5
				_dataSource.ReplaceContents(ConfigParameter.All.Select(c => (IConfigParameter)c));
			}
			catch (Exception ex)
			{
				ErrorReport.ReportError("Reading configuration", ex);
				ErrorCommand.Navigate();
			}
		}

		private void HandleRefreshCommand(object sender, EventArgs e)
		{
			ConfigParameter.Storage.Refresh();
			Refresh();
		}

		private void HandleEditCommand(object sender, EventArgs e)
		{
			TaskState.ConfigParameter = View.Current;
			if (TaskState.ConfigParameter == null)
			{
				return;
			}
			EditCommand.Navigate();
		}
	}
}