using System;
using System.Windows.Forms;
using Quokka.Config;
using Quokka.Config.Storage;
using Quokka.UI.Commands;
using Quokka.UI.Config;
using Quokka.Util;
using Quokka.WinForms.Commands;

namespace Quokka.WinForms.Config
{
	public partial class ListConfigView : UserControl, IListConfigView
	{
		private readonly VirtualDataGridViewAdapter<IConfigParameter> _adapter;
		private readonly DisplaySettings _displaySettings = new DisplaySettings(typeof (ListConfigView));

		public ListConfigView()
		{
			InitializeComponent();
			_adapter = new VirtualDataGridViewAdapter<IConfigParameter>(DataGridView)
				.WithDisplaySettings(_displaySettings)
				.DefineCellValue(NameColumn, p => p.Name)
				.DefineCellValue(ParameterTypeColumn, p => p.ParameterType)
				.DefineCellValue(ValueColumn, p => p.GetValueText())
				.DefineCellValue(DescriptionColumn, p => p.Description)
				.SetDefaultSortOrder(NameColumn)
				.SortBy(NameColumn);

			EditCommand = new UICommand(EditButton);
		}

		public IUICommand EditCommand { get; private set; }

		public IVirtualDataSource<IConfigParameter> DataSource
		{
			set { _adapter.Init(value); }
		}

		public IConfigParameter Current
		{
			get { return _adapter.Current; }
		}

		private void DataGridViewCellDoubleClick(object sender, DataGridViewCellEventArgs e)
		{
			EditButton.PerformClick();
		}

		private void ClearSearchTextButtonClick(object sender, EventArgs e)
		{
			SearchTextBox.Text = string.Empty;
		}

		private void SearchTextBoxTextChanged(object sender, EventArgs e)
		{
			if (StringUtils.IsNullOrWhiteSpace(SearchTextBox.Text))
			{
				_adapter.DataSource.Filter = null;
			}
			else
			{
				_adapter.DataSource.Filter = new Filter(SearchTextBox.Text).Apply;
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Enter)
			{
				EditButton.PerformClick();
				return true;
			}
			if (keyData == Keys.Escape)
			{
				ClearSearchTextButton.PerformClick();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}


		private class Filter
		{
			private readonly string _text;

			public Filter(string text)
			{
				_text = (text ?? string.Empty).Trim().ToLowerInvariant();
			}

			public bool Apply(IConfigParameter configParameter)
			{
				return configParameter.Name.ToLowerInvariant().Contains(_text) ||
				       configParameter.Description.ToLowerInvariant().Contains(_text);
			}
		}
	}
}