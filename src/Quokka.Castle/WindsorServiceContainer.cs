using System;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
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
			if (to == null)
			{
				to = from;
			}

			if (String.IsNullOrEmpty(name))
			{
				name = to.FullName;
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
				_container.Register(
					Component.For(type)
						.Named(type.FullName)
						.Instance(instance)
					);
			}
			else if (type == null)
			{
				_container.Register(
					Component.For<object>()
						.Named(name)
						.Instance(instance)
					);
			}
			else
			{
				_container.Register(
					Component.For(type)
						.Named(name)
						.Instance(instance)
					);
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

		protected override bool DoIsTypeRegistered(Type type)
		{
			return _container.Kernel.HasComponent(type);
		}
	}
}