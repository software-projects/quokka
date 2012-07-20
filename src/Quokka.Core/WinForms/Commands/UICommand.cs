﻿using System;
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

		static UICommand()
		{
			EnabledChangedEventArgs = new PropertyChangedEventArgs("Enabled");
			TextChangedEventArgs = new PropertyChangedEventArgs("Text");
			CheckChangedEventArgs = new PropertyChangedEventArgs("Checked");
		}

		public UICommand(Control control)
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

		public bool Checked
		{
			get
			{
				if (_checkControl == null)
				{
					return false;
				}
				return PerformFunction(() => _checkControl.Checked);
			}

			set
			{
				if (CanCheck)
				{
					PerformAction(() => _checkControl.Checked = value);
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
			get { return PerformFunction(() => _control.Enabled); }
			set { PerformAction(() => _control.Enabled = value); }
		}

		public string Text
		{
			get { return PerformFunction(() => _control.Text); }
			set { PerformAction(() => _control.Text = value); }
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
			if (_control.InvokeRequired)
			{
				// Using a synchronization context here because the
				// Invoke method can fail if the control's underlying
				// window handle has not been created yet. It is not
				// possible to check for this, because accessing the
				// Handle property on a cross-thread throws an exception,
				// so synchronization context it is.
				_synchronizationContext.Send(state => action(), null);
			}
			else
			{
				action();
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