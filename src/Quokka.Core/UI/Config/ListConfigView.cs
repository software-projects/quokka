﻿using System;
using Quokka.Config;
using Quokka.UI.Tasks;
using Quokka.WinForms;

namespace Quokka.UI.Config
{
	public class ListConfigView : Presenter<IListConfigView>
	{
		public INavigateCommand ErrorCommand { get; set; }
		public INavigateCommand EditCommand { get; set; }
		public ErrorReport ErrorReport { get; set; }
		public ConfigTaskState TaskState { get; set; }
		public IConfigRepo ConfigRepo { get; set; }

		private readonly VirtualDataSource<string, ConfigParameter> _dataSource =
			new VirtualDataSource<string, ConfigParameter>(p => p.Name);

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
				_dataSource.ReplaceContents(list);
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