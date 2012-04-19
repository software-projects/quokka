using System;
using System.ComponentModel;
using System.Threading;
using System.Windows.Forms;
using Quokka.DynamicCodeGeneration;
using Quokka.UI.Commands;
using Quokka.WinForms.Internal;

namespace Quokka.WinForms.Commands
{
	public class UICommand : IUICommand, INotifyPropertyChanged
	{
		private readonly Control _control;
		private readonly ICheckControl _checkControl;
		private readonly SynchronizationContext _synchronizationContext;
		private static readonly PropertyChangedEventArgs EnabledChangedEventArgs;
		private static readonly PropertyChangedEventArgs TextChangedEventArgs;
		private static readonly PropertyChangedEventArgs CheckChangedEventArgs;

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler Execute;
		public event CancelEventHandler Validating;
		public event EventHandler Validated;

		private string _text;
		private bool _checked;
		private bool _enabled;

		static UICommand()
		{
			EnabledChangedEventArgs = new PropertyChangedEventArgs("Enabled");
			TextChangedEventArgs = new PropertyChangedEventArgs("Text");
			CheckChangedEventArgs = new PropertyChangedEventArgs("Checked");
		}

		public UICommand(Control control = null)
		{
			_synchronizationContext = SynchronizationContext.Current;
			if (control == null)
			{
				_text = string.Empty;
			}
			else
			{
				if (control.InvokeRequired)
				{
					throw new InvalidOperationException("UICommand needs to be created on the same thread that created the control");
				}
				_control = control;
				_control.EnabledChanged += ControlEnabledChanged;
				_control.TextChanged += ControlTextChanged;
				_control.Click += ControlClick;
				_text = _control.Text;
				_enabled = _control.Enabled;

				// We are getting NREs and it seems to be coming from this code (happens inside the VS2010 designer
				// so it is difficult to debug). Try catching and ignoring error here to see if the problem in the
				// designer goes away.
				// TODO: come up with a better way to handle check controls, or find why this is throwing NREs.
				try
				{
					var checkControl = ProxyFactory.CreateDuckProxy<ICheckControl>(control);
					if (checkControl.IsCheckedSupported)
					{
						_checkControl = checkControl;
						if (checkControl.IsCheckedChangedSupported)
						{
							_checkControl.CheckedChanged += ControlCheckedChanged;
						}
						_checked = _checkControl.Checked;
					}
				}
				catch (NullReferenceException ex)
				{
					_checkControl = null;
					_checked = false;
				}
			}
		}

		public bool Checked
		{
			get { return _checked; }

			set
			{
				if (_checkControl != null)
				{
					PerformAction(() => _checkControl.Checked = value);
				}
				else
				{
					if (_checked != value)
					{
						_checked = value;
						RaisePropertyChanged("Checked");
					}
				}
			}
		}

		public bool Enabled
		{
			get { return _enabled; }
			set
			{
				if (_control != null)
				{
					PerformAction(() => _control.Enabled = value);
				}
				else
				{
					if (_enabled != value)
					{
						_enabled = value;
						RaisePropertyChanged("Enabled");
					}
				}
			}
		}

		public string Text
		{
			get { return _text; }
			set
			{
				// never allow null text, convert to empty text
				if (value == null)
				{
					value = string.Empty;
				}
				if (_control != null)
				{
					PerformAction(() => _control.Text = value);
				}
				else
				{
					if (_text != value)
					{
						_text = value;
						RaisePropertyChanged("Text");
					}
				}
			}
		}

		public void PerformExecute()
		{
			if (Validating != null)
			{
				var cancelEventArgs = new CancelEventArgs();
				PerformAction(() => Validating(this, cancelEventArgs));
				if (cancelEventArgs.Cancel)
				{
					return;
				}
			}

			if (Validated != null)
			{
				PerformAction(() => Validated(this, EventArgs.Empty));
			}

			if (Execute != null)
			{
				PerformAction(() => Execute(this, EventArgs.Empty));
			}
		}

		protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
		{
			if (PropertyChanged != null)
			{
				PerformAction(() => PropertyChanged(this, e));
			}
		}

		private void RaisePropertyChanged(string propertyName)
		{
			if (PropertyChanged != null)
			{
				var e = new PropertyChangedEventArgs(propertyName);
				OnPropertyChanged(e);
			}
		}

		private void ControlEnabledChanged(object sender, EventArgs e)
		{
			_enabled = _control.Enabled;
			OnPropertyChanged(EnabledChangedEventArgs);
		}

		private void ControlTextChanged(object sender, EventArgs e)
		{
			_text = _control.Text;
			OnPropertyChanged(TextChangedEventArgs);
		}


		private void ControlCheckedChanged(object sender, EventArgs e)
		{
			_checked = _checkControl.Checked;
			OnPropertyChanged(CheckChangedEventArgs);
		}

		private void ControlClick(object sender, EventArgs e)
		{
			PerformExecute();
		}

		private void PerformAction(Action action)
		{
			// Using a synchronization context here because the
			// Invoke method can fail if the control's underlying
			// window handle has not been created yet. It is not
			// possible to check for this, because accessing the
			// Handle property on a cross-thread throws an exception,
			// so synchronization context it is.
			if (_synchronizationContext == null)
			{
				action();
			}
			else
			{
				_synchronizationContext.Send(state => action(), null);
			}
		}
	}
}