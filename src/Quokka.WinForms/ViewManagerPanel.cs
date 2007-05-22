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

using Quokka.Uip;

namespace Quokka.WinForms
{
    public class ViewManagerPanel : Panel, IUipViewManager 
    {
        private List<UipTask> currentTasks = new List<UipTask>();
    	private List<Form> modalForms = new List<Form>();

        public event EventHandler AllTasksComplete;

        #region IUipViewManager Members

        public event EventHandler<UipViewEventArgs> ViewClosed;

        public void BeginTask(UipTask task) {
            if (!currentTasks.Contains(task)) {
                currentTasks.Add(task);
            }
        }

        public void EndTask(UipTask task) {
            currentTasks.Remove(task);
            if (currentTasks.Count == 0) {
                OnAllTasksComplete(EventArgs.Empty);
            }
        }

        public void BeginTransition() {
            SuspendLayout();
            WindowUtil.SetWindowRedraw(this, false);
            Cursor.Current = Cursors.WaitCursor;
        }

        public void EndTransition() {
            Cursor.Current = Cursors.Default;
            WindowUtil.SetWindowRedraw(this, true);
            Invalidate(true);
            ResumeLayout();
        }

        public void AddView(object view, object controller) {
            // view object may optionally be a Form, but it must be a control
            Form form = view as Form;
            Control control = (Control)view;

            if (form != null) {
                form.TopLevel = false;
                form.FormBorderStyle = FormBorderStyle.None;
                form.Closed += new EventHandler(View_Closed);
            }

            control.Dock = DockStyle.Fill;
            control.Visible = false;
            Controls.Add(control);

            // This gives a chance for all of the controls within the view control
            // to get a look at the controller. To get the controller, the contained
            // control must implement a method called "SetController", which accepts
            // a compatible type. (Could be an embedded "IController" interface.
            WinFormsUipUtil.SetController(control, controller);
        }

    	private void View_Closed(object sender, EventArgs e)
    	{
			Form form = sender as Form;
			if (form != null) {
				modalForms.Remove(form);
			}

    		UipViewEventArgs eventArgs = new UipViewEventArgs(sender);
    		OnViewClosed(eventArgs);
    	}

    	public void RemoveView(object view) {
            Control control = (Control)view;

    		Form form = view as Form;
			if (form != null && modalForms.Contains(form)) {
				// this is a modal form -- close it
				form.Close();
			}
			else {
				Controls.Remove(control);
			}
    	}

        public void ShowView(object view) {
            foreach (Control c in Controls) {
                c.Visible = false;
            }
            Control control = (Control)view;
            control.Visible = true;
        }

        public void HideView(object view) {
            Control control = (Control)view;
            control.Visible = false;
        }

		public void ShowModalView(object view, object controller)
		{
			Form form = view as Form;
			if (form == null) {
				throw new ArgumentException("Modal views must inherit from System.Windows.Forms.Form");
			}

			WinFormsUipUtil.SetController(form, controller);
			modalForms.Add(form);
			form.Closed += new EventHandler(View_Closed);
			form.ShowDialog(this.TopLevelControl);
		}

        #endregion

        protected virtual void OnAllTasksComplete(EventArgs e) {
            if (AllTasksComplete != null) {
                AllTasksComplete(this, e);
            }
        }

		protected virtual void OnViewClosed(UipViewEventArgs e)
		{
			if (ViewClosed != null) {
				ViewClosed(this, e);
			}
		}
    }
}
