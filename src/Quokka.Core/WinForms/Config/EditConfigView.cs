using System;
using System.Windows.Forms;
using Quokka.Config;
using Quokka.Config.Storage;
using Quokka.UI.Commands;
using Quokka.UI.Config;
using Quokka.WinForms.Commands;

namespace Quokka.WinForms.Config
{
	public partial class EditConfigView : UserControl, IEditConfigView
	{
		private IConfigParameterEditor _editor;
		private IConfigParameter _configParameter;

		public EditConfigView()
		{
			InitializeComponent();
			SaveCommand = new UICommand(SaveButton);
			CancelCommand = new UICommand(CancelButton);
		}

		public IUICommand SaveCommand { get; private set; }
		public IUICommand CancelCommand { get; private set; }

		public IConfigParameter Parameter
		{
			get { return _configParameter; }
			set
			{
				_configParameter = value;
				NameValueLabel.Text = _configParameter.Name;
				TypeValueLabel.Text = _configParameter.ParameterType;
				DescriptionTextBox.Text = _configParameter.Description;
				if (_configParameter.IsReadOnly)
				{
					HeadingLabel.Text = "View Read-only Configuration Parameter";
					SaveButton.Visible = false;
					CancelButton.Text = "Close";
				}
				CreateAndAddEditorControl();
			}
		}

		public string Value
		{
			get
			{
				if (_editor == null)
				{
					return null;
				}
				return _editor.TextValue;
			}
		}

		protected virtual IConfigParameterEditor Editor
		{
			get { return _editor; }
		}

		private void CreateAndAddEditorControl()
		{
			SuspendLayout();
			EditValuePanel.Controls.Clear();
			_editor = null;
			if (_configParameter != null)
			{
				_editor = CreateEditor();
			}
			if (_editor != null)
			{
				var control = _editor.Control;
				EditValuePanel.Controls.Add(control);
				control.Dock = DockStyle.Fill;
				_editor.Initialize(_configParameter);
				BeginInvoke(new Action(() => control.Focus()));
			}
			ResumeLayout();
		}

		protected virtual IConfigParameterEditor CreateEditor()
		{
			if (Parameter.IsReadOnly)
			{
				return new ReadOnlyViewer();
			}

			switch (Parameter.ParameterType)
			{
				case ConfigParameterType.Int32:
					return new StringEditor {
						CharCount = 8,
					};
				case ConfigParameterType.Password:
					return new StringEditor {
						CharCount = 32,
					};
				case ConfigParameterType.Directory:
				case ConfigParameterType.FilePath:
					// don't specify size and it will be as big as possible.
					return new StringEditor();
				default:
					return new StringEditor();
			}
		}

		protected override bool ProcessDialogKey(Keys keyData)
		{
			if (keyData == Keys.Enter)
			{
				SaveButton.PerformClick();
				return true;
			}
			if (keyData == Keys.Escape)
			{
				CancelButton.PerformClick();
				return true;
			}
			return base.ProcessDialogKey(keyData);
		}
	}
}