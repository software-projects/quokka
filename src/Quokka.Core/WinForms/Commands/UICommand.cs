using System;
using System.ComponentModel;
using System.Threading;
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
		private readonly SynchronizationContext _synchronizationContext;
		private static readonly PropertyChangedEventArgs EnabledChangedEventArgs;
		private static readonly PropertyChangedEventArgs TextChangedEventArgs;
		private static readonly PropertyChangedEventArgs CheckChangedEventArgs;

		public event PropertyChangedEventHandler PropertyChanged;
		public event EventHandler Execute;
		public event CancelEventHandler Validating;
		public event EventHandler Validated;

		// property values when no control is supplied
		private string _noControlText;
		private bool _noControlEnabled;
		private bool _noControlChecked;

		static UICommand()
		{
			EnabledChangedEventArgs = new PropertyChangedEventArgs("Enabled");
			TextChangedEventArgs = new PropertyChangedEventArgs("Text");
			CheckChangedEventArgs = new PropertyChangedEventArgs("Checked");
		}

		public UICommand(Control control = null)
		{
			if (control == null)
			{
				_noControlText = string.Empty;
				_noControlEnabled = false;
				_noControlChecked = false;
			}
			else
			{
				if (control.InvokeRequired)
				{
					throw new InvalidOperationException("UICommand needs to be created on the same thread that created the control");
				}
				_synchronizationContext = SynchronizationContext.Current;
				_control = Verify.ArgumentNotNull(control, "control");
				_control.EnabledChanged += ControlEnabledChanged;
				_control.TextChanged += ControlTextChanged;
				_control.Click += ControlClick;

				// We are getting NREs and it seems to be coming from this code (happens inside the VS2010 designer
				// so it is difficult to debug). Try catching and ignoring error here to see if the problem in the
				// designer goes away.
				// TODO: come up with a better way to handle check controls, or find why this is throwing NREs.
				try
				{
					var checkControl = ProxyFactory.CreateDuckProxy<ICheckControl>(control);
					if (checkControl != null && checkControl.IsCheckedSupported)
					{
						_checkControl = checkControl;
						if (checkControl.IsCheckedChangedSupported)
						{
							_checkControl.CheckedChanged += ControlCheckedChanged;
						}
					}
				}
				catch (NullReferenceException)
				{
					_checkControl = null;
				}
			}
		}

		public bool Checked
		{
			get
			{
				if (_checkControl == null)
				{
					return _noControlChecked;
				}
				return PerformFunction(() => _checkControl.Checked);
			}

			set
			{
				if (_checkControl == null)
				{
					if (_noControlChecked != value)
					{
						_noControlChecked = value;
						OnPropertyChanged(CheckChangedEventArgs);
					}
				}
				else
				{
					PerformAction(() => _checkControl.Checked = value);
				}
			}
		}

		public bool CanCheck
		{
			get { return _checkControl != null; }
		}

		public bool Enabled
		{
			get
			{
				if (_control == null)
				{
					return _noControlEnabled;
				}

				return PerformFunction(() => _control.Enabled);
			}
			set
			{
				if (_control == null)
				{
					if (_noControlEnabled != value)
					{
						_noControlEnabled = value;
						OnPropertyChanged(EnabledChangedEventArgs);
					}
				}
				else
				{
					PerformAction(() => _control.Enabled = value);
				}
			}
		}

		public string Text
		{
			get
			{
				if (_control == null)
				{
					return _noControlText;
				}
				return PerformFunction(() => _control.Text);
			}
			set
			{
				if (_control == null)
				{
					if (_noControlText != value)
					{
						_noControlText = value;
						OnPropertyChanged(TextChangedEventArgs);
					}
				}
				else
				{
					PerformAction(() => _control.Text = value);
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

		private void PerformAction(Action action)
		{
			if (_synchronizationContext == null)
			{
				action();
			}
			else
			{
				_synchronizationContext.Send(state => action(), null);
			}
		}

		private T PerformFunction<T>(Func<T> func)
		{
			T result = default(T);
			Action action = () => result = func();
			PerformAction(action);
			return result;
		}
	}
}