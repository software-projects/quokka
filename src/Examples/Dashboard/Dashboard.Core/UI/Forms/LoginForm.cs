using System;
using System.Drawing;
using ComponentFactory.Krypton.Toolkit;
using Dashboard.UI.Forms.Interfaces;

namespace Dashboard.UI.Forms
{
	public partial class LoginForm : KryptonForm, ILoginForm
	{
		public LoginForm()
		{
			InitializeComponent();
			loginPanel.Visible = false;
			CenterPanel();
			SizeChanged += delegate { CenterPanel(); };
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
			set { errorProvider.SetError(usernameTextBox, value);}
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