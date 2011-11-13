using System;
using System.Windows.Forms;

namespace Quokka.Server.Internal
{
	/// <summary>
	/// User control that is displayed when an exception occurs very early in the program startup.
	/// </summary>
	/// <remarks>
	/// VERY IMPORTANT: This form should not depend on any third-party or any other assembly other
	/// than the .NET CLR assemblies. This means only controls from System.Windows.Forms. The problem
	/// may very well be that the third-party assembly is missing, and that would prevent this form
	/// from displaying.
	/// </remarks>
	public partial class CannotStartError : UserControl
	{
		public CannotStartError()
		{
			InitializeComponent();
			textBox.Text = "No diagnostic information available.";
		}

		public CannotStartError(Exception ex) : this()
		{
			SetError(ex);
		}

		public void SetError(object obj)
		{
			if (obj != null)
			{
				var anotherInstanceRunningException = obj as AnotherInstanceRunningException;
				if (anotherInstanceRunningException != null)
				{
					textBox.Text = anotherInstanceRunningException.Message;
				}
				else
				{
					textBox.Text = obj.ToString();
				}
			}
			textBox.Select(0, 0);
		}

		private void CopyLinkLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Clipboard.SetText(textBox.Text);
			MessageBox.Show(this, "Diagnostic text has been copied to the clipboard.", "Copied to Clipboard",
			                MessageBoxButtons.OK, MessageBoxIcon.Information);
		}

		private void FormLoad(object sender, EventArgs e)
		{
			Text = Application.ProductName;
		}
	}
}