using System;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using Quokka.Diagnostics;
using Quokka.UI.Tasks;

namespace Quokka.WinForms
{
	public class ModalWindow<TForm, TPanel> : IModalWindow
		where TForm : Form, new()
		where TPanel : Control, new()
	{
		private readonly TForm _form;
		private readonly TPanel _panel;
		private readonly MyViewDeck _viewDeck;
		private readonly SynchronizationContext _syncContext = new WindowsFormsSynchronizationContext();

		public event EventHandler Closed;

		public ModalWindow()
		{
			_form = new TForm();
			_form.Closed += (sender, e) => OnClosed(e);
			_panel = new TPanel();
			InitializeForm();
			_viewDeck = new MyViewDeck(_panel, _form);
		}

		public void Dispose()
		{
			_form.Dispose();
		}

		public TForm Form
		{
			get { return _form; }
		}

		public TPanel Panel
		{
			get { return _panel; }
		}

		public IViewDeck ViewDeck
		{
			get { return _viewDeck; }
		}

		public IWin32Window Owner { get; set; }

		public void ShowModal()
		{
			_syncContext.Post(ShowDialog, null);
		}

		protected virtual void OnClosed(EventArgs e)
		{
			if (Closed != null)
			{
				Closed(this, e);
			}
		}

		private void ShowDialog(object state)
		{
			if (Owner == null)
			{
				_form.ShowDialog();
			}
			else
			{
				_form.ShowDialog(Owner);
			}
		}

		private void InitializeForm()
		{
			_form.SuspendLayout();
			// 
			// _panel
			// 
			_panel.Dock = DockStyle.Fill;
			_panel.Location = new Point(0, 0);
			_panel.Name = "_viewDeckPanel";
			_panel.Visible = true;
			_panel.TabIndex = 0;
			// 
			// _form
			// 
			_form.AutoScaleDimensions = new SizeF(6F, 13F);
			_form.AutoScaleMode = AutoScaleMode.Font;
			_form.ClientSize = new Size(480, 640);
			_form.Controls.Add(_panel);
			_form.Name = "ModalForm";
			_form.Text = Application.ProductName;
			_form.StartPosition = FormStartPosition.CenterParent;
			_form.MinimizeBox = false;
			_form.MaximizeBox = false;
			_form.ResumeLayout(false);
		}

		/// <summary>
		/// 	Subclass ViewDeck to handle sizing of the form when the first view is added.
		/// </summary>
		private class MyViewDeck : ViewDeck
		{
			private bool _viewAdded;
			private readonly Form _form;

			public MyViewDeck(Control control, Form form) : base(control)
			{
				_form = Verify.ArgumentNotNull(form, "form");
			}

			public override void AddView(object view)
			{
				if (!_viewAdded)
				{
					var control = (Control) view;
					_form.ClientSize = control.Size;
					if (control.MinimumSize == control.MaximumSize 
						&& control.MinimumSize.Width > 0
						&& control.MinimumSize.Height > 0)
					{
						// take this as a hint that the form border should be fixed
						_form.FormBorderStyle = FormBorderStyle.FixedDialog;
					}
				}
				base.AddView(view);
				_viewAdded = true;
			}
		}
	}
}