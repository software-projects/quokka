using System;
using System.Windows.Forms;
using Quokka.Config;
using Quokka.Diagnostics;

namespace Quokka.WinForms.Config
{
	public partial class StringEditor : UserControl, IConfigParameterEditor
	{
		/// <summary>
		/// Desired width in characters.
		/// </summary>
		public int CharCount { get; set; }

		public StringEditor()
		{
			InitializeComponent();
			Load += OnLoad;
		}

		private void OnLoad(object sender, EventArgs eventArgs)
		{
			if (CharCount > 0)
			{
				textBox.Width = TextRenderer.MeasureText(new string('m', CharCount), textBox.Font).Width;
			}
		}

		public Control Control
		{
			get { return this; }
		}

		public ConfigParameter Parameter { get; private set; }

		public void Initialize(ConfigParameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			Parameter = parameter;
			textBox.Text = parameter.GetValueText();
		}

		public string TextValue
		{
			get { return textBox.Text; }
		}

		private void StringEditorSizeChanged(object sender, EventArgs e)
		{
			textBox.Width = Width - textBox.Left*2;
		}
	}
}