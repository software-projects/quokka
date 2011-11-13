using Castle.Windsor;
using Quokka.ServiceLocation;

namespace Quokka.Server.Internal
{
	public static class ServiceContainerFactory
	{
		public static IServiceContainer CreateContainer()
		{
			IWindsorContainer windsorContainer = new WindsorContainer();
			IServiceContainer serviceContainer = new WindsorServiceContainer(windsorContainer);
			serviceContainer.RegisterInstance(windsorContainer);
			return serviceContainer;
		}
	}
}