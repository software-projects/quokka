using System.Windows.Forms;
using Quokka.WinForms.Startup;

namespace Example1
{
	public class Bootstrapper : BootstrapperBase
	{
		protected override Form CreateShell()
		{
			MainForm form = new MainForm();
			WelcomeTask task = new WelcomeTask();
			task.Start(form.ViewManager);
			return form;
		}
	}
}