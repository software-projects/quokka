using System;
using System.Reflection;
using System.Windows.Forms;
using ComponentFactory.Krypton.Toolkit;
using Dashboard.Services;
using Dashboard.Services.Interfaces;
using Dashboard.UI.Forms;
using Dashboard.UI.Tasks;
using Quokka.Krypton;
using Quokka.ServiceLocation;
using Quokka.Castle;
using Quokka.WinForms;
using Quokka.WinForms.Startup;

namespace Dashboard
{
	public class Bootstrapper : BootstrapperBase
	{
		protected override Form CreateShell()
		{
			MainForm mainForm = Locator.GetInstance<MainForm>();

			// TODO: this can go in module initialization
			LoginTask task = new LoginTask();
			task.Start(mainForm.ViewDeck);
			return mainForm;
		}

		protected override IServiceContainer CreateServiceContainer()
		{
			return ServiceContainerFactory.CreateContainer();
		}

		protected override void ConfigureServices()
		{
			Container.RegisterTypesInAssembly(Assembly.GetExecutingAssembly());
			Container.RegisterType<IModalWindowFactory, KryptonModalWindowFactory>(ServiceLifecycle.Singleton);
		}
	}
}