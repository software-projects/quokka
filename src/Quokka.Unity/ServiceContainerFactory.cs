using Microsoft.Practices.Unity;
using Quokka.ServiceLocation;

namespace Quokka.Unity
{
	public static class ServiceContainerFactory
	{
		public static IServiceContainer CreateContainer()
		{
			IUnityContainer unityContainer = new UnityContainer();
			IServiceContainer serviceContainer = new UnityServiceContainer(unityContainer);
			return serviceContainer;
		}
	}
}