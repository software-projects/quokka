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
using System.Windows.Forms;
using Quokka.Config;
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
				.DefineCellValue(DescriptionColumn, p => p.Summary)
				.SetDefaultSortOrder(NameColumn)
				.SortBy(NameColumn);

			EditCommand = new UICommand(EditButton);
			RefreshCommand = new UICommand(RefreshButton);

			Load += delegate {
				BeginInvoke(new Action(() => SearchTextBox.Focus()));
				DataGridView.ClearSelection();
				DataGridView.CurrentCell = null;
			};
		}

		public IUICommand EditCommand { get; private set; }
		public IUICommand RefreshCommand { get; private set; }

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

		private void DataGridViewMoveUp(object sender, EventArgs e)
		{
			DataGridView.ClearSelection();
			DataGridView.CurrentCell = null;
			SearchTextBox.Focus();
		}

		private void DataGridViewEnterKeyPressed(object sender, EventArgs e)
		{
			EditButton.PerformClick();
		}

		private void SearchTextBoxDownKeyPressed(object sender, EventArgs e)
		{
			DataGridView.Focus();
			if (DataGridView.SelectedRows.Count > 0 && DataGridView.SelectedRows[0].Selected)
				SelectGridRow(1); // since first row is already highlighted
			else
				SelectGridRow(0);
		}

		private void SearchTextBoxEnterKeyPressed(object sender, EventArgs e)
		{
			EditButton.PerformClick();
		}

		public void SelectGridRow(int rowIndex)
		{
			// If row is greater than number of rows then set to the last row.
			if (rowIndex >= DataGridView.Rows.Count)
				rowIndex = DataGridView.Rows.Count - 1;

			if (rowIndex < 0)
			{
				DataGridView.ClearSelection();
				DataGridView.CurrentCell = null;
			}
			else
			{
				DataGridView.Rows[rowIndex].Selected = true;
				DataGridView.CurrentCell = DataGridView[0, rowIndex];
			}
		}
	}
}