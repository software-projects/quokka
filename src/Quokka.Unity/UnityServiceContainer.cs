using System;
using Common.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;

namespace Quokka.Unity
{
	internal class UnityServiceContainer : ServiceContainer
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private readonly IUnityContainer _container;
		private readonly IServiceLocator _locator;

		public UnityServiceContainer(IUnityContainer container)
		{
			Verify.ArgumentNotNull(container, "container", out _container);
			_locator = new UnityServiceLocator(container);
			_container.AddNewExtension<ContainerExtension>();
		}

		protected override IServiceLocator GetServiceLocator()
		{
			return _locator;
		}

		protected override void DoRegisterType(Type from, Type to, string name, ServiceLifecycle lifecycle)
		{
			_container.RegisterType(from, to, name, CreateLifetimeManager(lifecycle));
		}

		protected override void DoRegisterInstance(Type type, string name, object instance)
		{
			_container.RegisterInstance(type, name, instance);
		}

		protected override IServiceContainer DoCreateChildContainer()
		{
			IUnityContainer childContainer = _container.CreateChildContainer();
			return new UnityServiceContainer(childContainer);
		}

		protected override void DoDispose()
		{
			_container.Dispose();
		}

		protected override bool DoIsTypeRegistered(Type type, string name)
		{
			return ContainerExtension.IsTypeRegistered(_container, type, name);
		}

		private static LifetimeManager CreateLifetimeManager(ServiceLifecycle lifecycle)
		{
			switch (lifecycle)
			{
				case ServiceLifecycle.PerRequest:
					return new TransientLifetimeManager();
				case ServiceLifecycle.Singleton:
					return new ContainerControlledLifetimeManager();
			}

			// This would happen if additional values are added to the enumeration without
			// modifying this code.
			const string format = "Unknown ServiceLifecycle value: {0}. Probable cause is that a new value has been added"
			                      +
			                      " to the ServiceLifecycle enumeration without changing the UnityServiceContainer class as well.";
			string message = String.Format(format, lifecycle);
			log.Error(message);
			throw new ArgumentException(message);
		}
	}
}