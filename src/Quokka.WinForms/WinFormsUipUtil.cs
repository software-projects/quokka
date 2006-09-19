using System;
using System.Windows.Forms;
using Quokka.Uip;

namespace Quokka.WinForms
{
    public static class WinFormsUipUtil
    {
        /// <summary>
        /// Attempt to set the controller for a WinForms control, and all its contained controls
        /// </summary>
        /// <param name="control">The control (may have any level of contained controls)</param>
        /// <param name="controller">The controller to assign.</param>
        public static void SetController(Control control, object controller) {
            if (control == null)
                return;
            UipUtil.SetController(control, controller, false);
            foreach (Control containedControl in control.Controls) {
                SetController(containedControl, controller);
            }
        }
    }
}
