using System;
using Castle.Core;
using Castle.MicroKernel;
using Castle.Windsor;
using Common.Logging;
using Microsoft.Practices.ServiceLocation;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;

namespace Quokka.Castle
{
	internal class WindsorServiceContainer : ServiceContainer
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private readonly IWindsorContainer _container;
		private readonly IServiceLocator _locator;

		public WindsorServiceContainer(IWindsorContainer container)
		{
			Verify.ArgumentNotNull(container, "container", out _container);
			_locator = new WindsorServiceLocator(container);
		}

		protected override IServiceLocator GetServiceLocator()
		{
			return _locator;
		}

		protected override void DoRegisterType(Type from, Type to, string name, ServiceLifecycle lifecycle)
		{
			if (String.IsNullOrEmpty(name))
			{
				name = from.FullName;
			}
			_container.AddComponentLifeStyle(name, from, to, MapLifecycle(lifecycle));
		}

		private static LifestyleType MapLifecycle(ServiceLifecycle lifecycle)
		{
			switch (lifecycle)
			{
				case ServiceLifecycle.PerRequest:
					return LifestyleType.Transient;
				case ServiceLifecycle.Singleton:
					return LifestyleType.Singleton;
			}

			throw new ArgumentException("Unknown ServiceLifecycle: " + lifecycle);
		} 

		protected override void DoRegisterInstance(Type type, string name, object instance)
		{
			if (String.IsNullOrEmpty(name))
			{
				_container.Kernel.AddComponentInstance(type.FullName, type, instance);
			}
			else if (type == null)
			{
				_container.Kernel.AddComponentInstance(name, instance);
			}
			else
			{
				_container.Kernel.AddComponentInstance(name, type, instance);
			}
		}

		protected override IServiceContainer DoCreateChildContainer()
		{
			IWindsorContainer childContainer = new WindsorContainer();
			_container.AddChildContainer(childContainer);
			return new WindsorServiceContainer(childContainer);
		}

		protected override void DoDispose()
		{
			_container.Dispose();
		}

		protected override bool DoIsTypeRegistered(Type type, string name)
		{
			try
			{
				// TODO: this is not very good, as it actually creates the type
				// Need to find out how to do this in Castle Windsor properly.
				if (name == null)
				{
					_container.Resolve(type);
				}
				else
				{
					_container.Resolve(name, type);
				}
				return true;
			}
			catch (ComponentNotFoundException)
			{
				return false;
			}
		}

	}
}