using System.Drawing;
using System.Windows.Forms;
using Microsoft.Practices.ServiceLocation;

namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// Implements a region that is shown as a modal dialog box.
	/// </summary>
	/// <typeparam name="TForm">Class used as the outside windows form.</typeparam>
	/// <typeparam name="TPanel">Control used to fill the client area of the <see typeparamref="TForm"/> form.</typeparam>
	public class ModalRegion<TForm, TPanel> : Region
		where TForm : Form
		where TPanel : Control
	{
		public IWin32Window ParentWindow { get; set; }

		protected override Control CreateHostControl()
		{
			Control control = ServiceLocator.Current.GetInstance<TPanel>();
			control.ControlAdded += HostControl_ControlAdded;
			return control;
		}

		protected override void OnAdd(RegionItem item)
		{
			string text = item.Text;
			if (string.IsNullOrEmpty(text))
			{
				text = Application.ProductName;
			}

			Form form = CreateForm();
			form.Text = text;
			form.ClientSize = item.HostControl.Size;
			form.MinimumSize = item.HostControl.MinimumSize; // TODO: need to account for form borders here
			form.Controls.Add(item.HostControl);
			item.HostControl.Dock = DockStyle.Fill;
			item.HostControl.Visible = true;
			form.ShowDialog(ParentWindow);
			form.FormClosed += FormClosed;
			item.Tag = form;
		}

		protected virtual TForm CreateForm()
		{
			TForm form = ServiceLocator.Current.GetInstance<TForm>();
			form.StartPosition = FormStartPosition.CenterParent;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.ShowIcon = false;
			form.ShowInTaskbar = false;
			return form;
		}

		/// <summary>
		/// Called the first time a control is added to the panel. Resize the panel to fit.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private static void HostControl_ControlAdded(object sender, ControlEventArgs e)
		{
			Control panel = sender as Control;
			if (panel == null)
			{
				// should not happen
				return;
			}

			if (e.Control.MinimumSize != default(Size))
			{
				panel.Size = e.Control.MinimumSize;
				panel.MinimumSize = e.Control.MinimumSize;
				panel.ControlAdded -= HostControl_ControlAdded;
			}
		}

		private void FormClosed(object sender, FormClosedEventArgs e)
		{
			// TODO: what do we do here
		}

		protected override void OnRemove(RegionItem item)
		{
			Form form = item.Tag as Form;
			if (form == null)
			{
				return;
			}

			if (!form.IsDisposed)
			{
				if (form.IsHandleCreated)
				{
					form.Close();
				}
				form.Dispose();
			}
		}
	}
}