using System;
using System.Windows.Forms;
using Example2.Tasks.Simple;
using Microsoft.Practices.ServiceLocation;
using Quokka.ServiceLocation;
using Quokka.Unity;
using Quokka.WinForms.Startup;

namespace Example2
{
	public class Bootstrapper : BootstrapperBase
	{
		protected override IServiceContainer CreateServiceContainer()
		{
			return ServiceContainerFactory.CreateContainer();
		}

		protected override Form CreateShell()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			ServiceLocator.SetLocatorProvider(() => container.Locator);

			MainForm form = new MainForm();
			form.MainRegion.Add(new SimpleTask());
			return form;
		}
	}
}