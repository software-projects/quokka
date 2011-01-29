using System;
using System.Windows.Forms;
using Dashboard.UI.Views.Interfaces;
using Quokka.ServiceLocation;
using Quokka.UI.Commands;
using Quokka.WinForms.Commands;

namespace Dashboard.UI.Views
{
	[PerRequest(typeof (IShellView))]
	public partial class ShellView : UserControl, IShellView
	{
		public ShellView()
		{
			InitializeComponent();
			logoutButton.Click += (sender, args) => RaiseLogout();
			modalButton.Click += (sender, args) => RaiseDoSomething();
		}

		public event EventHandler Logout;
		public event EventHandler DoSomething;

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

		private void RaiseDoSomething()
		{
			if (DoSomething != null)
			{
				DoSomething(this, EventArgs.Empty);
			}
		}
	}
}