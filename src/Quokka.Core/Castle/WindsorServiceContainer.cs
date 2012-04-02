using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Quokka.Diagnostics;
using Quokka.ServiceLocation;

namespace Quokka.Castle
{
	public class WindsorServiceContainer : ServiceContainer
	{
		private readonly IWindsorContainer _container;
		private readonly IServiceLocator _locator;
		private readonly Func<IWindsorContainer> _createChildCallback; 

		public WindsorServiceContainer(IWindsorContainer container, Func<IWindsorContainer> createChildCallback)
		{
			_container = Verify.ArgumentNotNull(container, "container");
			_locator = new WindsorServiceLocator(container);
			_createChildCallback = Verify.ArgumentNotNull(createChildCallback, "createChildCallback");
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

			// ReSharper disable ConvertIfStatementToConditionalTernaryExpression
			if (lifecycle == ServiceLifecycle.Singleton)
			{
				_container.Register(Component.For(from).ImplementedBy(to).Named(name).LifestyleSingleton());
			}
			else
			{
				_container.Register(Component.For(from).ImplementedBy(to).Named(name).LifestyleTransient());
			}
			// ReSharper restore ConvertIfStatementToConditionalTernaryExpression
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
			IWindsorContainer childContainer = _createChildCallback();
			_container.AddChildContainer(childContainer);
			return new WindsorServiceContainer(childContainer, _createChildCallback);
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