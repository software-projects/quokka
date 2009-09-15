using System;
using System.Windows.Forms;
using Dashboard.UI.Forms.Interfaces;

namespace Dashboard.UI.Forms
{
	public partial class ShellForm : UserControl, IShellForm
	{
		public ShellForm()
		{
			InitializeComponent();
			logoutButton.Click += delegate { RaiseLogout(); };
		}

		public event EventHandler Logout;

		public string Username
		{
			get { return loginNameLabel.Text; }
			set { loginNameLabel.Text = value; }
		}

		private void RaiseLogout()
		{
			if (Logout != null)
			{
				Logout(this, EventArgs.Empty);
			}
		}
	}
}