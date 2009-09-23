using System.Windows.Forms;
using Example3.Tasks.Simple;
using Microsoft.Practices.ServiceLocation;
using Quokka.Castle;
using Quokka.ServiceLocation;
using Quokka.WinForms.Startup;

namespace Example3
{
	public class BootStrapper : BootstrapperBase
	{
		protected override IServiceContainer CreateServiceContainer()
		{
			return ServiceContainerFactory.CreateContainer();
		}

		protected override Form CreateShell()
		{
			MainForm form = ServiceLocator.Current.GetInstance<MainForm>();
			form.NavigatorRegion.Add(new SimpleTask());
			return form;
		}
	}
}