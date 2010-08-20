using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Quokka.Diagnostics;
using Quokka.UI.Tasks;
using Quokka.Uip;

namespace Quokka.WinForms
{
	/// <summary>
	/// Handles views inside a Windows Forms <see cref="Control"/>
	/// </summary>
	/// <remarks>
	/// Views are arranged in a 'deck' view, so that only one is visible at a time.
	/// </remarks>
	public class ViewDeck : IViewDeck
	{
		private readonly Control _control;
		private readonly List<object> _currentTasks = new List<object>();
		private readonly List<Form> _modalForms = new List<Form>();
		private Control _currentVisibleView;
		private readonly List<Control> _visibleViews = new List<Control>();

		public event EventHandler AllTasksComplete;

		public ViewDeck(Control control)
		{
			_control = Verify.ArgumentNotNull(control, "control");
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

		public event EventHandler<ViewClosedEventArgs> ViewClosed;

		public void BeginTask(object task)
		{
			if (!_currentTasks.Contains(task))
			{
				_currentTasks.Add(task);
			}
		}

		public void EndTask(object task)
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

		public void AddView(object view)
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
		}

		private void View_Closed(object sender, EventArgs e)
		{
			Form form = sender as Form;
			if (form != null)
			{
				_modalForms.Remove(form);
			}

			ViewClosedEventArgs eventArgs = new ViewClosedEventArgs(sender);
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

		public void ShowModalView(object view)
		{
			Form form = view as Form;
			if (form == null)
			{
				throw new ArgumentException("Modal views must inherit from System.Windows.Forms.Form");
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

		protected virtual void OnViewClosed(ViewClosedEventArgs e)
		{
			if (ViewClosed != null)
			{
				ViewClosed(this, e);
			}
		}
	}
}
