using System;
using System.Windows.Forms;

using Quokka.Uip;

namespace Quokka.WinForms
{
    public class ViewManagerPanel : Panel, IUipViewManager 
    {
        #region IUipViewManager Members

        public event EventHandler<UipViewEventArgs> ViewClosed;

        public void BeginTransition() {
            SuspendLayout();
            WindowUtil.SetWindowRedraw(this, false);
        }

        public void EndTransition() {
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
    }
}
