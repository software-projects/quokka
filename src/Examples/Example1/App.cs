using System.Windows.Forms;
using Quokka.ServiceLocation;
using Quokka.Unity;
using Microsoft.Practices.ServiceLocation;

namespace Example1
{
	public static class App
	{
		public static Form CreateMainForm()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			ServiceLocator.SetLocatorProvider(() => container.Locator);
			MainForm form = new MainForm();
			WelcomeTask task = new WelcomeTask();
			task.Start(form.ViewManager);
			return form;
		}
	}
}