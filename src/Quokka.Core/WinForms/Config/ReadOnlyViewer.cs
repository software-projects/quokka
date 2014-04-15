using System;
using System.Drawing;
using System.Windows.Forms;
using Quokka.Config;
using Quokka.Diagnostics;

namespace Quokka.WinForms.Config
{
	public partial class ReadOnlyViewer : UserControl, IConfigParameterEditor
	{
		private Control _userControlParent;

		/// <summary>
		/// Desired width in characters.
		/// </summary>
		public int CharCount { get; set; }

		public ReadOnlyViewer()
		{
			InitializeComponent();
			Load += OnLoad;
		}

		private void OnLoad(object sender, EventArgs eventArgs)
		{
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
			textBox.Text = parameter.GetValueText();
			// because we cannot edit, we don't want to select the contents
			textBox.Select(0, 0);
		}

		public string TextValue
		{
			get { return textBox.Text; }
		}

		private void UserControlSizeChanged(object sender, EventArgs e)
		{
			if (_userControlParent != null)
			{
				var parentLocation = _userControlParent.PointToScreen(new Point(0, 0));
				var textBoxLocation = textBox.PointToScreen(new Point(0, 0));
				var margin = textBoxLocation.X - parentLocation.X;
				// TODO: need to use system metrics for this
				textBox.Width = _userControlParent.Width - margin - 40;
			}
		}
	}
}