using System;
using System.Windows.Forms;
using Quokka.WinForms;
using log4net.Config;
using log4net.Core;
using log4net.Repository;
using log4net.Repository.Hierarchy;

namespace Quokka.Server.Internal
{
	public partial class LogView : UserControl
	{
		private readonly DisplaySettings _displaySettings = new DisplaySettings(typeof(LogView));
		private readonly VirtualDataGridViewAdapter<LoggingEvent> _adapter;
		private readonly EventLoggingDataSource<LoggingEvent> _dataSource = new EventLoggingDataSource<LoggingEvent>();

		public event EventHandler StopRequested;

		public LogView()
		{
			InitializeComponent();

			_adapter = new VirtualDataGridViewAdapter<LoggingEvent>(dataGridView)
				.DefineCellValue(timestampColumn, l => l.TimeStamp)
				.DefineCellValue(levelColumn, l => l.Level.DisplayName)
				.DefineCellValue(loggerColumn, l => l.LoggerName)
				.DefineCellValue(messageColumn, l => l.RenderedMessage)
				.WithDisplaySettings(_displaySettings);

			_adapter.Init(_dataSource);
			_adapter.ListChanged += (o, e) => ListChanged();

			var appender = new DisplayAppender(_dataSource);

			// Don't show NHibernate debug logs as they are very verbose
			appender.AddFilter(new SuppressDebugFilter { LoggerToMatch = "NHibernate" });

			// Don't show Quartz debug logs as they are very verbose
			appender.AddFilter(new SuppressDebugFilter { LoggerToMatch = "Quartz" });

			appender.ActivateOptions();

			ILoggerRepository repository = log4net.LogManager.GetRepository();

			var hierarchy = repository as Hierarchy;
			if (hierarchy == null)
			{
				return;
			}

			if (hierarchy.Configured)
			{
				hierarchy.Root.AddAppender(appender);
			}
			else
			{
				BasicConfigurator.Configure(hierarchy, appender);
			}
		}

		private void ListChanged()
		{
			foreach (DataGridViewCell cell in dataGridView.SelectedCells)
			{
				cell.Selected = false;
			}

			// when the number of rows changes, select the last row
			if (dataGridView.RowCount > 0)
			{
				DataGridViewRow row = dataGridView.Rows[dataGridView.RowCount - 1];

				// setting the current cell ensures that the data grid view row is visible on the grid
				dataGridView.CurrentCell = row.Cells[0];

				// set row as the selected row selected
				row.Selected = true;
			}
		}

		private void StopButtonClick(object sender, EventArgs e)
		{
			if (StopRequested != null)
			{
				StopRequested(this, EventArgs.Empty);
			}
		}

	}
}
