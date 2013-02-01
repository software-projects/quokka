using System;
using System.Windows.Forms;
using Quokka.Properties;

namespace Quokka.WinForms.Startup
{
	/// <summary>
	/// Form that is displayed when an exception occurs very early in the program startup.
	/// </summary>
	/// <remarks>
	/// VERY IMPORTANT: This form should not depend on any third-party or any other assembly other
	/// than the .NET CLR assemblies. This means only controls from System.Windows.Forms. The problem
	/// may very well be that the third-party assembly is missing, and that would prevent this form
	/// from displaying.
	/// </remarks>
	public partial class ErrorForm : Form
	{
		public ErrorForm()
		{
			InitializeComponent();
		}

		public ErrorForm(Exception ex) : this()
		{
			SetError(ex);
		}

		public ErrorForm(string errorMessage) : this()
		{
			SetError(errorMessage);
		}

		public void SetError(object obj)
		{
			if (obj != null)
			{
				textBox.Text = obj.ToString();
			}
			textBox.Select(0, 0);
		}

		private void CloseButtonClick(object sender, EventArgs e)
		{
			Close();
		}

		private void CopyLinkLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Clipboard.SetText(textBox.Text);
			MessageBox.Show(this, Resources.DiagnosticInfoCopiedToClipboard, ApplicationInfo.MessageBoxCaption,
			                MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void FormLoad(object sender, EventArgs e)
		{
			Text = ApplicationInfo.ProductName;
			if (ApplicationInfo.Icon != null)
			{
				Icon = ApplicationInfo.Icon;
			}
		}
	}
}