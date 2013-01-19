using System;
using System.Linq;
using Quokka.Config;
using Quokka.Config.Storage;
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
		public IConfigRepo ConfigRepo { get; set; }

		private readonly VirtualDataSource<string, IConfigParameter> _dataSource =
			new VirtualDataSource<string, IConfigParameter>(p => p.Name);

		public override void InitializePresenter()
		{
			View.EditCommand.Execute += HandleEditCommand;
			View.DataSource = _dataSource;
			Refresh();
		}

		private void Refresh()
		{
			try
			{
				var list = ConfigRepo.FindAll();

				// casting is necessary for .NET 3.5
				_dataSource.ReplaceContents(list.Select(c => (IConfigParameter)c));
			}
			catch (Exception ex)
			{
				ErrorReport.ReportError("Reading configuration", ex);
				ErrorCommand.Navigate();
			}
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