using System;
using System.Drawing;
using System.Windows.Forms;
using Quokka.Config;
using Quokka.Config.Storage;
using Quokka.Diagnostics;

namespace Quokka.WinForms.Config
{
	public partial class DirectoryEditor : UserControl, IConfigParameterEditor
	{
		private Control _userControlParent;

		/// <summary>
		/// Desired width in characters.
		/// </summary>
		public int CharCount { get; set; }

		public DirectoryEditor()
		{
			InitializeComponent();

			// makes editing easier during design time, but we want no borders at runtime
			BorderStyle = BorderStyle.None;

			Load += OnLoad;
		}

		private void OnLoad(object sender, EventArgs eventArgs)
		{
			// size not specified, use all available space
			var parent = Parent;
			while (parent != null && !(parent is UserControl))
			{
				parent = parent.Parent;
			}

			if (parent != null)
			{
				parent.SizeChanged += UserControlSizeChanged;
				_userControlParent = parent;
			}
		}

		public Control Control
		{
			get { return this; }
		}

		public IConfigParameter Parameter { get; private set; }

		public void Initialize(IConfigParameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			Parameter = parameter;
			TextBox.Text = parameter.GetValueText();
		}

		public string TextValue
		{
			get { return TextBox.Text; }
		}

		private void UserControlSizeChanged(object sender, EventArgs e)
		{
			if (_userControlParent != null)
			{
				var parentLocation = _userControlParent.PointToScreen(new Point(0, 0));
				var textBoxLocation = TextBox.PointToScreen(new Point(0, 0));
				var margin = textBoxLocation.X - parentLocation.X;
				// TODO: need to use system metrics for this
				TextBox.Width = _userControlParent.Width - TableLayoutPanel.GetColumnWidths()[1] - margin - 40;
			}
		}

		protected virtual void BrowseButtonClick(object sender, EventArgs e)
		{
			var dialog = new FolderBrowserDialog();
			dialog.Description = Parameter.Name;
			dialog.ShowNewFolderButton = true;
			dialog.SelectedPath = Parameter.GetValueText();
			if (dialog.ShowDialog(TopLevelControl) == DialogResult.OK)
			{
				TextBox.Text = dialog.SelectedPath;
			}
		}
	}
}