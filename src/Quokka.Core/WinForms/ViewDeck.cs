#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using Castle.DynamicProxy;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;
using Quokka.UI.Tasks;

namespace Quokka.WinForms
{
	/// <summary>
	/// 	Handles views inside a Windows Forms <see cref="Control" />
	/// </summary>
	/// <remarks>
	/// 	Views are arranged in a 'deck' view, so that only one is visible at a time.
	/// </remarks>
	public class ViewDeck : IViewDeck
	{
		private readonly Control _control;
		private int _transitionReferenceCount;
		private readonly HashSet<UITask> _currentUITasks = new HashSet<UITask>();
		protected Control CurrentVisibleView;
		protected readonly List<Control> VisibleViews = new List<Control>();
		private readonly HashSet<Control> _viewControls = new HashSet<Control>();

		public event EventHandler AllTasksComplete;

		public ViewDeck(Control control)
		{
			_control = Verify.ArgumentNotNull(control, "control");
			_control.Disposed += ControlDisposed;
			EndTasksWhenDisposed = true;
		}

		public Control Control
		{
			get { return _control; }
		}

		/// <summary>
		/// Determines whether any tasks that have views displayed should be ended when
		/// the control is disposed. Usually set to <c>true</c>, but will be <c>false</c>
		/// for modal view decks.
		/// </summary>
		public bool EndTasksWhenDisposed { get; set; }

		public virtual void Clear()
		{
			// Remove all the view controls that we added.
			// (Leaving behind any controls that we did not create).
			// The controls were created in the service container, and the
			// container will dispose of them.
			foreach (var control in _viewControls)
			{
				_control.Controls.Remove(control);
			}

			_viewControls.Clear();
			_currentUITasks.Clear();
			VisibleViews.Clear();
			CurrentVisibleView = null;
		}

		#region IViewDeck Members

		public event EventHandler<ViewClosedEventArgs> ViewClosed;

		public virtual IViewTransition BeginTransition(UITask task)
		{
			if (task != null)
			{
				if (!_currentUITasks.Contains(task))
				{
					_currentUITasks.Add(task);
					task.TaskComplete += TaskCompleteHandler;
					BeginTask(task);
				}
			}

			BeginTransition();
			return new ViewTransition(this);
		}

		private void TaskCompleteHandler(object sender, EventArgs e)
		{
			var task = sender as UITask;
			if (task != null)
			{
				task.TaskComplete -= TaskCompleteHandler;
				_currentUITasks.Remove(task);
				EndTask(task);

				if (_currentUITasks.Count == 0)
				{
					OnAllTasksComplete(EventArgs.Empty);
				}
			}
		}

		public virtual IModalWindow CreateModalWindow()
		{
			var factory = ServiceLocator.Current.GetInstance<IModalWindowFactory>();
			try
			{
				var window = factory.CreateModalWindow(Control);
				return window;
			}
			finally
			{
				ServiceLocator.Current.Release(factory);
			}
		}

		#endregion

		#region Public methods for IUipViewManager backwards compatibility (now protected virtuals)

		protected virtual void BeginTask(UITask task)
		{
		}

		protected virtual void EndTask(UITask task)
		{
		}

		protected virtual void BeginTransition()
		{
			// Use reference counts, because as of Quokka 0.6, it is possible for 
			// calls to this method to be nested. (This happens when nested tasks are
			// defined in the node of a task).
			if (Interlocked.Increment(ref _transitionReferenceCount) == 1)
			{
				Cursor.Current = Cursors.WaitCursor;

				// The framework should not call this method when the view deck control
				// is disposed or disposing, but check just in case.
				if (!_control.IsDisposed && !_control.Disposing)
				{
					_control.SuspendLayout();
					Win32.SetWindowRedraw(_control, false);
				}
			}
		}

		protected virtual void EndTransition()
		{
			// Use reference counts, because as of Quokka 0.6, it is possible for 
			// calls to this method to be nested. (This happens when nested tasks are
			// defined in the node of a task).
			if (Interlocked.Decrement(ref _transitionReferenceCount) == 0)
			{
				Cursor.Current = Cursors.Default;

				// This method can be called after the view deck control has been disposed at the
				// end of a task. Check for disposed or disposing control and do nothing if this
				// is the case.
				if (!_control.IsDisposed && !_control.Disposing)
				{
					if (CurrentVisibleView == null && VisibleViews.Count > 0)
					{
						// At the end of the transition, no view is visible but there
						// are one or more views that are not visible because they were
						// hidden in order to display another view, but they were never
						// commanded to be hidden. Show them in reverse order.
						ShowView(VisibleViews[VisibleViews.Count - 1]);
					}

					Win32.SetWindowRedraw(_control, true);
					_control.Invalidate(true);
					_control.ResumeLayout();
				}
			}
		}

		protected virtual void AddView(object view)
		{
			if (_control.IsDisposed || _control.Disposing || view == null)
			{
				return;
			}

			// view object may optionally be a Form, but it must be a control
			Control control = GetControl(view);
			var form = control as Form;

			if (form != null)
			{
				form.TopLevel = false;
				form.FormBorderStyle = FormBorderStyle.None;
				form.Closed += ViewClosedHandler;
			}

			control.Dock = DockStyle.Fill;
			control.Visible = false;
			_viewControls.Add(control);
			_control.Controls.Add(control);
		}

		private void ViewClosedHandler(object sender, EventArgs e)
		{
			var eventArgs = new ViewClosedEventArgs(sender);
			OnViewClosed(eventArgs);
		}

		protected virtual void RemoveView(object view)
		{
			if (_control.IsDisposed || _control.Disposing || view == null)
			{
				return;
			}

			Control control = GetControl(view);
			HideView(view);
			_viewControls.Remove(control);
			_control.Controls.Remove(control);
		}

		protected virtual void ShowView(object view)
		{
			if (_control.IsDisposed || _control.Disposing || view == null)
			{
				return;
			}

			Control control = GetControl(view);
			control.Visible = true;
			foreach (Control c in _viewControls)
			{
				if (!ReferenceEquals(c, control))
				{
					c.Visible = false;
					VisibleViews.Remove(c);
				}
			}

			if (!VisibleViews.Contains(control))
			{
				VisibleViews.Add(control);
			}
			CurrentVisibleView = control;
		}

		protected virtual void HideView(object view)
		{
			if (_control.IsDisposed || _control.Disposing || view == null)
			{
				return;
			}

			Control control = GetControl(view);
			control.Visible = false;
			if (CurrentVisibleView == control)
			{
				CurrentVisibleView = null;
			}
			VisibleViews.Remove(control);
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

		protected virtual void TryEndTask(UITask task)
		{
			if (task != null && task.IsRunning)
			{
				task.EndTask();
			}
		}

		protected virtual Control GetControl(object view)
		{
			var proxy = view as IProxyTargetAccessor;
			if (proxy != null)
			{
				view = proxy.DynProxyGetTarget();
			}

			var control = view as Control;
			if (control == null)
			{
				var msg = string.Format("The ViewDeck is expecting a view to be of type System.Windows.Forms.Control"
				                        + ", but it is of type {0}", view.GetType());
				throw new QuokkaException(msg);
			}

			return control;
		}

		#region Private methods

		private void ControlDisposed(object sender, EventArgs e)
		{
			if (EndTasksWhenDisposed)
			{
				if (_currentUITasks.Count > 0)
				{
					// Take a copy of the CurrentTasks collection.
					// This is necessary because we are going to end the tasks,
					// which will cause them to remove themselves from the CurrentTasks
					// collection. Because that collection is being modified, we cannot use
					// an enumerator on it.
					var tasks = new List<UITask>(_currentUITasks);
					foreach (var task in tasks)
					{
						TryEndTask(task);
					}
				}
			}
		}

		#endregion

		#region Private class ViewTransition

		private class ViewTransition : IViewTransition
		{
			private readonly ViewDeck _viewDeck;
			private bool _disposed;

			public ViewTransition(ViewDeck viewDeck)
			{
				_viewDeck = Verify.ArgumentNotNull(viewDeck, "viewDeck");
			}

			public void Dispose()
			{
				if (!_disposed)
				{
					_disposed = true;
					_viewDeck.EndTransition();
				}
			}

			public void AddView(object view)
			{
				CheckDisposed();
				_viewDeck.AddView(view);
			}

			public void RemoveView(object view)
			{
				CheckDisposed();
				_viewDeck.RemoveView(view);
			}

			public void ShowView(object view)
			{
				CheckDisposed();
				_viewDeck.ShowView(view);
			}

			public void HideView(object view)
			{
				CheckDisposed();
				_viewDeck.HideView(view);
			}

			private void CheckDisposed()
			{
				if (_disposed)
				{
					throw new ObjectDisposedException("Attempt to use a ViewDeck transition after it has been disposed");
				}
			}
		}

		#endregion
	}
}