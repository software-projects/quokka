using System.Drawing;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Dashboard.UI.Forms.Interfaces;

namespace Dashboard.UI.Forms
{
	public partial class LoggingInForm : UserControl, ILoggingInForm
	{
		public LoggingInForm()
		{
			InitializeComponent();
			loginPanel.Visible = false; // reduces flicker
			CenterPanel();
			SizeChanged += delegate { CenterPanel(); };
		}

		// Center the login panel in the form
		private void CenterPanel()
		{
			loginPanel.Location = new Point((Width - loginPanel.Width)/2, (Height - loginPanel.Height)/2);
			if (IsHandleCreated)
			{
				// delay visibility to reduce flicker
				loginPanel.Visible = true;
			}
		}
	}
}