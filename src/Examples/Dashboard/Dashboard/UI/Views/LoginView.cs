using System;
using System.Drawing;
using System.Windows.Forms;
using Dashboard.UI.Views.Interfaces;
using Quokka.ServiceLocation;

namespace Dashboard.UI.Views
{
	[PerRequest(typeof(ILoginView))]
	public partial class LoginView : UserControl, ILoginView
	{
		public LoginView()
		{
			InitializeComponent();
			loginPanel.Visible = false;
			CenterPanel();
			Load += delegate { CenterPanel(); };
			SizeChanged += delegate
			               	{
								if (ParentForm != null)
								{
									ParentForm.AcceptButton = loginButton;
								}
			               		CenterPanel();
			               	};
			loginButton.Click += delegate { RaiseLogin(); };
		}

		public event EventHandler Login;

		public string Username
		{
			get { return usernameTextBox.Text; }
			set { usernameTextBox.Text = value; }
		}

		public string Password
		{
			get { return passwordTextBox.Text; }
			set { passwordTextBox.Text = value; }
		}

		public string ErrorMessage
		{
			get { return errorProvider.GetError(usernameTextBox); }
			set { errorProvider.SetError(usernameTextBox, value); }
		}

		private void RaiseLogin()
		{
			if (Login != null)
			{
				Login(this, EventArgs.Empty);
			}
		}

		// Center the login panel in the form
		private void CenterPanel()
		{
			loginPanel.Location = new Point((Width - loginPanel.Width)/2, (Height - loginPanel.Height)/2);
			if (IsHandleCreated)
			{
				loginPanel.Visible = true;
			}
		}
	}
}