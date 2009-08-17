using System;
using System.Windows.Forms;
using Quokka.ServiceLocation;
using Quokka.Unity;
using Quokka.WinForms.Startup;

namespace Example1
{
	public class Bootstrapper : BootstrapperBase
	{
		protected override IServiceContainer CreateServiceContainer()
		{
			return ServiceContainerFactory.CreateContainer();
		}

		protected override Form CreateShell()
		{
			MainForm form = new MainForm();
			WelcomeTask task = new WelcomeTask();
			task.Start(form.ViewManager);
			return form;
		}
	}
}