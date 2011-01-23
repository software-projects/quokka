using System;
using System.Windows.Forms;
using Dashboard.UI.Views.Interfaces;
using Quokka.ServiceLocation;

namespace Dashboard.UI.Views
{
	[PerRequest(typeof(IShellView))]
	public partial class ShellView : UserControl, IShellView
	{
		public ShellView()
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