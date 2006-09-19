using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Quokka.Uip;

namespace Quokka.WinForms
{
    public class ViewManagerPanel : Panel, IUipViewManager 
    {
        private List<UipTask> currentTasks = new List<UipTask>();

        public event EventHandler AllTasksFinished;

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
                OnAllTasksFinished(EventArgs.Empty);
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
                //form.Closed += new EventHandler(view_Closed);
            }

            control.Dock = DockStyle.Fill;
            control.Visible = false;
            Controls.Add(control);

            // This gives a chance for all of the controls within the view control
            // to get a look at the controller. To get the controller, the contained
            // control must implement a method called "SetController", which accepts
            // a compatible type. (Could be an embedded "IController" interface.
            WinFormsUipUtil.SetController(control
                , controller);
        }

        public void RemoveView(object view) {
            Control control = (Control)view;
            Controls.Remove(control);
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

        #endregion

        protected virtual void OnAllTasksFinished(EventArgs e) {
            if (AllTasksFinished != null) {
                AllTasksFinished(this, e);
            }
        }
    }
}
