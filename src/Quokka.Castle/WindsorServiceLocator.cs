using System;
using System.Collections.Generic;
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Common.Logging;
using Microsoft.Practices.ServiceLocation;

namespace Quokka.Castle
{
	/// <summary>
	/// Adapts the behavior of the Windsor container to the common
	/// IServiceLocator
	/// </summary>
	public class WindsorServiceLocator : ServiceLocatorImplBase
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();
		private readonly IWindsorContainer container;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorServiceLocator"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		public WindsorServiceLocator(IWindsorContainer container)
		{
			this.container = container;
		}

		/// <summary>
		///             When implemented by inheriting classes, this method will do the actual work of resolving
		///             the requested service instance.
		/// </summary>
		/// <param name="serviceType">Type of instance requested.</param>
		/// <param name="key">Name of registered service you want. May be null.</param>
		/// <returns>
		/// The requested service instance.
		/// </returns>
		protected override object DoGetInstance(Type serviceType, string key)
		{
			try
			{
				// Currently, Windsor Container does not support the functionality of creating
				// concrete components without registering them first. Quokka (and Prism, for that matter)
				// rely on being able to do this.
				//
				// BTW I got the idea from the following post at stack overflow:
				// http://stackoverflow.com/questions/447193/resolving-classes-without-registering-them-using-castle-windsor
				if (serviceType.IsClass && !container.Kernel.HasComponent(serviceType))
				{
					container.Kernel.Register(Component.For(serviceType).LifeStyle.Transient);
				}

				if (key != null)
					return container.Resolve(key, serviceType);
				return container.Resolve(serviceType);
			}
			catch (OutOfMemoryException)
			{
				throw;
			}
			catch (AccessViolationException)
			{
				throw;
			}
			catch (ComponentNotFoundException ex)
			{
				string message = String.Format("Component not found: type={0}, key={1}", serviceType, key);
				throw new ActivationException(message, ex);
			}
			catch (Exception ex)
			{
				string message = String.Format("Failed to resolve component: type={0}, key={1}", serviceType, key);
				throw new ActivationException(message, ex);
			}
		}

		/// <summary>
		///             When implemented by inheriting classes, this method will do the actual work of
		///             resolving all the requested service instances.
		/// </summary>
		/// <param name="serviceType">Type of service requested.</param>
		/// <returns>
		/// Sequence of service instance objects.
		/// </returns>
		protected override IEnumerable<object> DoGetAllInstances(Type serviceType)
		{
			return (object[])container.ResolveAll(serviceType);
		}
	}
}