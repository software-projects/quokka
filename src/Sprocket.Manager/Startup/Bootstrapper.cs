using System.Windows.Forms;
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
        protected override IServiceContainer CreateServiceContainer()
        {
            return ServiceContainerFactory.CreateContainer();
        }

        protected override void ConfigureServices()
        {
            // register types in this assembly
            Container.RegisterTypesInAssembly(typeof (Bootstrapper).Assembly);

            Container.RegisterType<ISprocket,SprocketClient>(ServiceLifecycle.Singleton);
        }

        protected override Form CreateShell()
        {
            var form = Container.Locator.GetInstance<MainForm>();
            var task = Container.Locator.GetInstance<StartupTask>();
            task.Start(form.ViewDeck);
            return form;
        }
    }
}
