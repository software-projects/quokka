using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Windsor;
using Quokka.ServiceLocation;

namespace Quokka.Castle
{
	public static class ServiceContainerFactory
	{
		public static IServiceContainer CreateContainer()
		{
			var container = new WindsorContainer();
			return new WindsorServiceContainer(container, () => new WindsorContainer());
		}
	}
}
