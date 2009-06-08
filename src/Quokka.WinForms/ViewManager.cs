#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006 John Jeffery. All rights reserved.
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
using System.Windows.Forms;
using Quokka.Diagnostics;
using Quokka.Uip;

namespace Quokka.WinForms
{
	/// <summary>
	/// Handles views inside a Windows Forms <see cref="Control"/>
	/// </summary>
	/// <remarks>
	/// The <see cref="ViewManagerPanel"/> class duplicates a lot of this code, and should probably
	/// be obsolete now.
	/// </remarks>
	public class ViewManager : IUipViewManager
	{
		private readonly Control _control;
		private readonly List<UipTask> _currentTasks = new List<UipTask>();
		private readonly List<Form> _modalForms = new List<Form>();
		private Control _currentVisibleView;
		private readonly List<Control> _visibleViews = new List<Control>();

		public event EventHandler AllTasksComplete;

		public ViewManager(Control control)
		{
			Verify.ArgumentNotNull(control, "control", out _control);
		}

		public void Clear()
		{
			// Clear the controls displayed, but do not dispose of them.
			// The controls were created in the service container, and the
			// container will dispose of them.
			_control.Controls.Clear();

			_currentTasks.Clear();
			_modalForms.Clear();
			_visibleViews.Clear();
			_currentVisibleView = null;
		}

		public Control Control
		{
			get { return _control; }
		}

		#region IUipViewManager Members

		public event EventHandler<UipViewEventArgs> ViewClosed;

		public void BeginTask(UipTask task)
		{
			if (!_currentTasks.Contains(task))
			{
				_currentTasks.Add(task);
			}
		}

		public void EndTask(UipTask task)
		{
			_currentTasks.Remove(task);
			if (_currentTasks.Count == 0)
			{
				OnAllTasksComplete(EventArgs.Empty);
			}
		}

		public void BeginTransition()
		{
			_control.SuspendLayout();
			Win32.SetWindowRedraw(_control, false);
			Cursor.Current = Cursors.WaitCursor;
		}

		public void EndTransition()
		{
			if (_currentVisibleView == null && _visibleViews.Count > 0)
			{
				// At the end of the transition, no view is visible but there
				// are one or more views that are not visible because they were
				// hidden in order to display another view, but they were never
				// commanded to be hidden. Show them in reverse order.
				ShowView(_visibleViews[_visibleViews.Count - 1]);
			}

			Cursor.Current = Cursors.Default;
			Win32.SetWindowRedraw(_control, true);
			_control.Invalidate(true);
			_control.ResumeLayout();
		}

		public void AddView(object view, object controller)
		{
			if (view == null)
				return;

			// view object may optionally be a Form, but it must be a control
			Form form = view as Form;
			Control control = (Control)view;

			if (form != null)
			{
				form.TopLevel = false;
				form.FormBorderStyle = FormBorderStyle.None;
				form.Closed += View_Closed;
			}

			control.Dock = DockStyle.Fill;
			control.Visible = false;
			_control.Controls.Add(control);

			// This gives a chance for all of the controls within the view control
			// to get a look at the controller. To get the controller, the contained
			// control must implement a method called "SetController", which accepts
			// a compatible type. (Could be an embedded "IController" interface.
			if (controller != null)
			{
				WinFormsUipUtil.SetController(control, controller);
			}
		}

		private void View_Closed(object sender, EventArgs e)
		{
			Form form = sender as Form;
			if (form != null)
			{
				_modalForms.Remove(form);
			}

			UipViewEventArgs eventArgs = new UipViewEventArgs(sender);
			OnViewClosed(eventArgs);
		}

		public void RemoveView(object view)
		{
			if (view != null)
			{
				Control control = (Control)view;

				Form form = view as Form;
				if (form != null && _modalForms.Contains(form))
				{
					// this is a modal form -- close it
					form.Close();
				}
				else
				{
					HideView(view);
					_control.Controls.Remove(control);
				}
			}
		}

		public void ShowView(object view)
		{
			if (view != null)
			{
				Control control = (Control)view;
				control.Visible = true;
				foreach (Control c in _control.Controls)
				{
					if (!ReferenceEquals(c, control))
					{
						c.Visible = false;
					}
				}

				if (!_visibleViews.Contains(control))
				{
					_visibleViews.Add(control);
				}
				_currentVisibleView = control;
			}
		}

		public void HideView(object view)
		{
			if (view != null)
			{
				Control control = (Control)view;
				control.Visible = false;
				if (_currentVisibleView == control)
				{
					_currentVisibleView = null;
				}
				_visibleViews.Remove(control);
			}
		}

		public void ShowModalView(object view, object controller)
		{
			Form form = view as Form;
			if (form == null)
			{
				throw new ArgumentException("Modal views must inherit from System.Windows.Forms.Form");
			}

			if (controller != null)
			{
				WinFormsUipUtil.SetController(form, controller);
			}
			_modalForms.Add(form);
			form.Closed += View_Closed;
			form.ShowDialog(_control.TopLevelControl);
		}

		private delegate UipAnswer AskQuestionDelegate(UipQuestion question);

		public UipAnswer AskQuestion(UipQuestion question)
		{
			// This might be called from a different thread
			if (_control.InvokeRequired)
			{
				return (UipAnswer)_control.Invoke(new AskQuestionDelegate(AskQuestion), question);
			}

			MessageBoxForm questionForm = new MessageBoxForm {Question = question};
			questionForm.ShowDialog(_control.TopLevelControl);
			if (question.SelectedAnswer != null && question.SelectedAnswer.Callback != null)
			{
				question.SelectedAnswer.Callback();
			}
			return question.SelectedAnswer;
		}

		#endregion

		protected virtual void OnAllTasksComplete(EventArgs e)
		{
			if (AllTasksComplete != null)
			{
				AllTasksComplete(this, e);
			}
		}

		protected virtual void OnViewClosed(UipViewEventArgs e)
		{
			if (ViewClosed != null)
			{
				ViewClosed(this, e);
			}
		}
	}
}
