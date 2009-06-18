using System.Windows.Forms;
using Dashboard.Services;
using Dashboard.Services.Interfaces;
using Dashboard.UI.Forms;
using Dashboard.UI.Tasks.Shell;
using Quokka.ServiceLocation;
using Quokka.WinForms.Startup;

namespace Dashboard
{
	public class Bootstrapper : BootstrapperBase
	{
		protected override Form CreateShell()
		{
			MainForm mainForm = Locator.GetInstance<MainForm>();

			// TODO: this can go in module initialization
			ShellTask task = new ShellTask();
			mainForm.MainRegion.Add(task);
			mainForm.MainRegion.Activate(task);
			return mainForm;
		}

		protected override void ConfigureServices()
		{
			Container.RegisterType<ILoginService, LoginService>(ServiceLifecycle.Singleton);
		}
	}
}