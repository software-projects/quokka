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