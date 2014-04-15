using System.Windows.Forms;
using Castle.Windsor;
using Quokka.Castle;
using Quokka.ServiceLocation;
using Quokka.Sprocket;
using Quokka.WinForms.Startup;
using Sprocket.Manager.Tasks;
using Sprocket.Manager.Tasks.Startup;

namespace Sprocket.Manager.Startup
{
    public class Bootstrapper : BootstrapperBase
    {
        protected override IWindsorContainer CreateContainer(bool isChild)
        {
        	var container = new WindsorContainer();
        	return container;
        }

        protected override void ConfigureServices()
        {
            // register types in this assembly
            ServiceContainer.RegisterTypesInAssembly(typeof (Bootstrapper).Assembly);

            ServiceContainer.RegisterType<ISprocket,SprocketClient>(ServiceLifecycle.Singleton);
        }

        protected override Form CreateShell()
        {
            var form = ServiceContainer.Locator.GetInstance<MainForm>();
            var task = ServiceContainer.Locator.GetInstance<StartupTask>();
            task.Start(form.ViewDeck);
            return form;
        }
    }
}
