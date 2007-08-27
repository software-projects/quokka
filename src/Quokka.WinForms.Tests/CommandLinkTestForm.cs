namespace Quokka.WinForms.Tests
{
	using System;
	using System.Windows.Forms;

	public partial class CommandLinkTestForm : Form
	{
		public CommandLinkTestForm()
		{
			InitializeComponent();
		}

		private void modalDialogCommandLink_Click(object sender, EventArgs e)
		{
			MessageBox.Show("Dialog Box", "Test Program");
			messageLabel.Text = "Modal dialog button clicked";
		}

		private void defaultCommandLink_Click(object sender, EventArgs e)
		{
			messageLabel.Text = "default button clicked";
		}

		private void commandLink1_Click(object sender, EventArgs e)
		{
			messageLabel.Text = "Command link 1 clicked";
		}
	}
}