using System.Reflection;
using System.Windows.Forms;
using Castle.Windsor;
using Dashboard.UI.Forms;
using Dashboard.UI.Tasks;
using Quokka.Krypton;
using Quokka.ServiceLocation;
using Quokka.WinForms;
using Quokka.WinForms.Startup;

namespace Dashboard
{
	public class Bootstrapper : BootstrapperBase
	{
		protected override Form CreateShell()
		{
			var mainForm = Locator.GetInstance<MainForm>();

			var task = new LoginTask();
			task.Start(mainForm.ViewDeck);
			return mainForm;
		}

		protected override IWindsorContainer CreateContainer(bool isChild)
		{
			var container = new WindsorContainer();
			// TODO: can add facilities here
			return container;
		}

		protected override void ConfigureServices()
		{
			ServiceContainer.RegisterTypesInAssembly(Assembly.GetExecutingAssembly());
			ServiceContainer.RegisterType<IModalWindowFactory, KryptonModalWindowFactory>(ServiceLifecycle.Singleton);
		}
	}
}