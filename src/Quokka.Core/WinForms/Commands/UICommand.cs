using System;
using System.ComponentModel;
using System.Windows.Forms;
using Quokka.Diagnostics;
using Quokka.DynamicCodeGeneration;
using Quokka.UI.Commands;
using Quokka.WinForms.Internal;

namespace Quokka.WinForms.Commands
{
	public class UICommand : IUICommand, INotifyPropertyChanged
	{
		private readonly Control _control;
		private readonly ICheckControl _checkControl;
		private static readonly PropertyChangedEventArgs EnabledChangedEventArgs;
		private static readonly PropertyChangedEventArgs TextChangedEventArgs;
		private static readonly PropertyChangedEventArgs CheckChangedEventArgs;

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler Execute;

		static UICommand()
		{
			EnabledChangedEventArgs = new PropertyChangedEventArgs("Enabled");
			TextChangedEventArgs = new PropertyChangedEventArgs("Text");
			CheckChangedEventArgs = new PropertyChangedEventArgs("Checked");
		}

		public UICommand(Control control)
		{
			_control = Verify.ArgumentNotNull(control, "control");
			_control.EnabledChanged += ControlEnabledChanged;
			_control.TextChanged += ControlTextChanged;
			_control.Click += ControlClick;

			var checkControl = ProxyFactory.CreateDuckProxy<ICheckControl>(control);
			if (checkControl.IsCheckedSupported)
			{
				_checkControl = checkControl;
				if (checkControl.IsCheckedChangedSupported)
				{
					_checkControl.CheckedChanged += ControlCheckedChanged;
				}
			}
		}

		public bool Checked
		{
			get
			{
				if (_checkControl == null)
				{
					return false;
				}
				return _checkControl.Checked;
			}

			set
			{
				if (CanCheck)
				{
					_checkControl.Checked = value;
				}
				else
				{
					throw new NotSupportedException("Checked property is not supported");
				}
			}
		}

		public bool CanCheck
		{
			get { return _checkControl != null; }
		}

		public bool Enabled
		{
			get { return _control.Enabled; }
			set { _control.Enabled = value; }
		}

		public string Text
		{
			get { return _control.Text; }
			set { _control.Text = value; }
		}

		public void PerformExecute()
		{
			if (Execute != null)
			{
				Execute(this, EventArgs.Empty);
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, e);
			}
		}

		private void ControlEnabledChanged(object sender, EventArgs e)
		{
			OnPropertyChanged(EnabledChangedEventArgs);
		}

		private void ControlTextChanged(object sender, EventArgs e)
		{
			OnPropertyChanged(TextChangedEventArgs);
		}


		private void ControlCheckedChanged(object sender, EventArgs e)
		{
			OnPropertyChanged(CheckChangedEventArgs);
		}

		private void ControlClick(object sender, EventArgs e)
		{
			PerformExecute();
		}
	}
}