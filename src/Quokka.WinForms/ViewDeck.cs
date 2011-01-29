using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;
using Quokka.Diagnostics;
using Quokka.UI.Tasks;

namespace Quokka.WinForms
{
	/// <summary>
	/// 	Handles views inside a Windows Forms <see cref = "Control" />
	/// </summary>
	/// <remarks>
	/// 	Views are arranged in a 'deck' view, so that only one is visible at a time.
	/// </remarks>
	public class ViewDeck : IViewDeck
	{
		private readonly Control _control;
		protected readonly List<object> CurrentTasks = new List<object>();
		private readonly List<Form> _modalForms = new List<Form>();
		protected Control CurrentVisibleView;
		protected readonly List<Control> VisibleViews = new List<Control>();
		protected readonly SynchronizationContext SynchronizationContext = new WindowsFormsSynchronizationContext();


		public event EventHandler AllTasksComplete;

		public ViewDeck(Control control)
		{
			_control = Verify.ArgumentNotNull(control, "control");
		}

		public virtual void Clear()
		{
			// Clear the controls displayed, but do not dispose of them.
			// The controls were created in the service container, and the
			// container will dispose of them.
			_control.Controls.Clear();

			CurrentTasks.Clear();
			_modalForms.Clear();
			VisibleViews.Clear();
			CurrentVisibleView = null;
		}

		public Control Control
		{
			get { return _control; }
		}

		#region IUipViewManager Members

		public event EventHandler<ViewClosedEventArgs> ViewClosed;

		public void BeginTask(object task)
		{
			if (!CurrentTasks.Contains(task))
			{
				CurrentTasks.Add(task);
			}
		}

		public void EndTask(object task)
		{
			CurrentTasks.Remove(task);
			if (CurrentTasks.Count == 0)
			{
				OnAllTasksComplete(EventArgs.Empty);
			}
		}

		private int _transitionReferenceCount;

		public void BeginTransition()
		{
			// Use reference counts, because as of Quokka 0.6, it is possible for 
			// calls to this method to be nested. (This happens when nested tasks are
			// defined in the node of a task).
			if (Interlocked.Increment(ref _transitionReferenceCount) == 1)
			{
				_control.SuspendLayout();
				Win32.SetWindowRedraw(_control, false);
				Cursor.Current = Cursors.WaitCursor;
			}
		}

		public void EndTransition()
		{
			// Use reference counts, because as of Quokka 0.6, it is possible for 
			// calls to this method to be nested. (This happens when nested tasks are
			// defined in the node of a task).
			if (Interlocked.Decrement(ref _transitionReferenceCount) == 0)
			{
				if (CurrentVisibleView == null && VisibleViews.Count > 0)
				{
					// At the end of the transition, no view is visible but there
					// are one or more views that are not visible because they were
					// hidden in order to display another view, but they were never
					// commanded to be hidden. Show them in reverse order.
					ShowView(VisibleViews[VisibleViews.Count - 1]);
				}

				Cursor.Current = Cursors.Default;
				Win32.SetWindowRedraw(_control, true);
				_control.Invalidate(true);
				_control.ResumeLayout();
			}
		}

		public virtual void AddView(object view)
		{
			if (view == null)
				return;

			// view object may optionally be a Form, but it must be a control
			Form form = view as Form;
			Control control = (Control) view;

			if (form != null)
			{
				form.TopLevel = false;
				form.FormBorderStyle = FormBorderStyle.None;
				form.Closed += ViewClosedHandler;
			}

			control.Dock = DockStyle.Fill;
			control.Visible = false;
			_control.Controls.Add(control);
		}

		protected void ViewClosedHandler(object sender, EventArgs e)
		{
			ViewClosedEventArgs eventArgs = new ViewClosedEventArgs(sender);
			OnViewClosed(eventArgs);
		}

		public virtual void RemoveView(object view)
		{
			if (view != null)
			{
				Control control = (Control) view;
				HideView(view);
				_control.Controls.Remove(control);
			}
		}

		public virtual void ShowView(object view)
		{
			if (view != null)
			{
				Control control = (Control) view;
				control.Visible = true;
				foreach (Control c in _control.Controls)
				{
					if (!ReferenceEquals(c, control))
					{
						c.Visible = false;
					}
				}

				if (!VisibleViews.Contains(control))
				{
					VisibleViews.Add(control);
				}
				CurrentVisibleView = control;
			}
		}

		public virtual void HideView(object view)
		{
			if (view != null)
			{
				Control control = (Control) view;
				control.Visible = false;
				if (CurrentVisibleView == control)
				{
					CurrentVisibleView = null;
				}
				VisibleViews.Remove(control);
			}
		}

		public virtual IModalWindow CreateModalWindow()
		{
			var factory = ServiceLocator.Current.GetInstance<IModalWindowFactory>();
			var window = factory.CreateModalWindow(Control);
			return window;
		}

		#endregion

		protected virtual void OnAllTasksComplete(EventArgs e)
		{
			if (AllTasksComplete != null)
			{
				AllTasksComplete(this, e);
			}
		}

		protected virtual void OnViewClosed(ViewClosedEventArgs e)
		{
			if (ViewClosed != null)
			{
				ViewClosed(this, e);
			}
		}
	}
}