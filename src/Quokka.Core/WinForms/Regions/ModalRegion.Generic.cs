using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Quokka.ServiceLocation;

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
		public Control ParentWindow { get; set; }

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
			form.Controls.Add(item.HostControl);
			item.HostControl.Dock = DockStyle.Fill;
			item.HostControl.Visible = true;
			form.FormClosed += FormClosed;
			form.Tag = item;
			item.Tag = form;
			item.PropertyChanged += item_PropertyChanged;
		}

		private void item_PropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RegionItem item = sender as RegionItem;
			if (item == null)
			{
				// should not happen
				return;
			}
			if (e.PropertyName == "IsActive" && item.IsActive)
			{
				Form form = item.Tag as Form;
				if (form == null)
				{
					// should not happen
					return;
				}

				Activate(form, item);
			}
		}

		private void Activate(Form form, RegionItem item)
		{
			form.ClientSize = item.HostControl.Size;
			if (item.HostControl.MinimumSize != default(Size) && item.HostControl.MinimumSize == item.HostControl.MaximumSize)
			{
				// the host control communicates its desire to stay the same size 
				form.FormBorderStyle = FormBorderStyle.FixedDialog;
			}
			else
			{
				// TODO: need to account for form borders here
				form.MinimumSize = item.HostControl.MinimumSize;
			}

			Action action = () => form.ShowDialog(ParentWindow);
			if (ParentWindow != null && ParentWindow.IsHandleCreated)
			{
				ParentWindow.BeginInvoke(action);
			}
			else
			{
				// this is not ideal, as it blocks the caller
				action();
			}
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
			panel.ControlAdded -= HostControl_ControlAdded;

			if (e.Control.MinimumSize != default(Size))
			{
				panel.Size = e.Control.MinimumSize;
				panel.MinimumSize = e.Control.MinimumSize;
			}
			if (e.Control.MaximumSize != default(Size))
			{
				panel.MaximumSize = e.Control.MaximumSize;
			}
		}

		private void FormClosed(object sender, FormClosedEventArgs e)
		{
			Form form = sender as Form;
			if (form == null)
			{
				// should not happen
				return;
			}

			RegionItem item = form.Tag as RegionItem;
			if (item == null)
			{
				// should not happen
				return;
			}

			item.IsActive = false;
			Remove(item.Item);
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