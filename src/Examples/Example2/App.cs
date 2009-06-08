using System.Windows.Forms;
using Example2.Tasks.Simple;
using Microsoft.Practices.ServiceLocation;
using Quokka.ServiceLocation;
using Quokka.Unity;

namespace Example2
{
	public static class App
	{
		public static Form CreateMainForm()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			ServiceLocator.SetLocatorProvider(() => container.Locator);

			MainForm form = new MainForm();
			form.MainWorkspace.StartTask(new SimpleTask());
			return form;
		}
	}
}