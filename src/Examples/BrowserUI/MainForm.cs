using System;
using System.Reflection;
using System.Windows.Forms;
using Quokka.UI.WebBrowsers;

namespace BrowserUI
{
	public partial class MainForm : Form
	{
		public MainForm()
		{
			InitializeComponent();
		}

		private void MainFormLoad(object sender, EventArgs e)
		{
			PluggableProtocol.Register(Assembly.GetExecutingAssembly());
			webBrowser.Navigate("quokka://host/index.html");
		}
	}
}
