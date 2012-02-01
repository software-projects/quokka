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