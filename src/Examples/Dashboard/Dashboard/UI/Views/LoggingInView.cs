using System.Drawing;
using System.Windows.Forms;
using Dashboard.UI.Views.Interfaces;
using Quokka.ServiceLocation;

namespace Dashboard.UI.Views
{
	[PerRequest(typeof(ILoggingInView))]
	public partial class LoggingInView : UserControl, ILoggingInView
	{
		public LoggingInView()
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