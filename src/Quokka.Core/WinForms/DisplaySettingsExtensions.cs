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

using System.Drawing;
using System.Windows.Forms;
using Quokka.Diagnostics;

namespace Quokka.WinForms
{
	public static class DisplaySettingsExtensions
	{
		public static void SaveColumnWidth(this DisplaySettings settings, DataGridViewColumn column)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(column, "column");

			if (column.AutoSizeMode == DataGridViewAutoSizeColumnMode.None
				|| column.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
			{
				string valueName = column.Name + ".ColumnWidth";
				int value = column.Width;
				settings.SetInt(valueName, value);
			}
		}

		public static void SaveColumnWidths(this DisplaySettings settings, DataGridView dataGridView)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(dataGridView, "dataGridView");

			foreach (DataGridViewColumn column in dataGridView.Columns)
			{
				settings.SaveColumnWidth(column);
			}
		}

		public static void LoadColumnWidths(this DisplaySettings settings, DataGridView dataGridView)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(dataGridView, "dataGridView");

			foreach (DataGridViewColumn column in dataGridView.Columns)
			{
				if (column.AutoSizeMode == DataGridViewAutoSizeColumnMode.None
					|| column.AutoSizeMode == DataGridViewAutoSizeColumnMode.NotSet)
				{
					string valueName = column.Name + ".ColumnWidth";
					int value = settings.GetInt(valueName, -1);
					if (value >= column.MinimumWidth)
					{
						column.Width = value;
					}
				}
			}
		}

		public static void SetSize(this DisplaySettings settings, string name, Size size)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(name, "name");
			settings.SetInt(name + ".Width", size.Width);
			settings.SetInt(name + ".Height", size.Height);
		}

		public static Size GetSize(this DisplaySettings settings, string name, Size defaultValue)
		{
			Verify.ArgumentNotNull(settings, "settings");
			Verify.ArgumentNotNull(name, "name");
			int height = settings.GetInt(name + ".Height", -1);
			int width = settings.GetInt(name + ".Width", -1);
			if (height < 0 || width < 0)
			{
				return defaultValue;
			}
			return new Size(width, height);
		}
	}
}