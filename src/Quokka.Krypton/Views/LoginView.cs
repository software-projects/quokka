using System;
using System.Drawing;
using System.Windows.Forms;
using Quokka.Krypton.ViewInterfaces;

namespace Quokka.Krypton.Views
{
	public partial class LoginForm : UserControl, ILoginView
	{
		public LoginForm()
		{
			InitializeComponent();
			loginPanel.Visible = false;
			CenterPanel();
			Load += delegate
			        	{
			        		if (ParentForm != null)
			        		{
			        			ParentForm.AcceptButton = loginButton;
			        		}
			        		CenterPanel();
			        	};
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
			get { return errorMessageLabel.Text; }
			set
			{
				errorMessageLabel.Text = value;
				errorMessagePanel.Visible = !string.IsNullOrEmpty(value);
			}
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
				errorMessagePanel.Location = new Point(loginPanel.Location.X, loginPanel.Location.Y + loginPanel.Size.Height + 10);
			}
		}
	}
}