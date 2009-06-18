using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Example3.Tasks.Simple;
using Microsoft.Practices.ServiceLocation;
using Quokka.ServiceLocation;
using Quokka.Unity;

namespace Example3
{
	public static class App
	{
		public static Form CreateMainForm()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			ServiceLocator.SetLocatorProvider(() => container.Locator);

			MainForm form = new MainForm();
			form.MainWorkspace.Add(new SimpleTask());
			return form;
		}
	}
}
