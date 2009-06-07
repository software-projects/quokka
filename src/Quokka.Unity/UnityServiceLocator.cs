using System;
using System.Collections.Generic;
using Common.Logging;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;

namespace Quokka.Unity
{
	/// <summary>
	/// Service locator adapter class for the unity container
	/// </summary>
	/// <remarks>
	/// This class is based on the Unity service locator adapter assembly, (Microsoft.Practices.ServiceLocation.UnityAdapter),
	/// but it makes use of logging to make it easier to diagnose container problems (ie fail to resolve type).
	/// </remarks>
	internal class UnityServiceLocator : ServiceLocatorImplBase
	{
		private readonly IUnityContainer container;
		private static readonly ILog logger = LogManager.GetCurrentClassLogger();

		public UnityServiceLocator(IUnityContainer container)
		{
			this.container = container;
		}

		/// <summary>
		/// When implemented by inheriting classes, this method will do the actual work of resolving
		/// the requested service instance.
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
				return container.Resolve(serviceType, key);
			}
			catch (OutOfMemoryException)
			{
				// don't want to do anything to these
				throw;
			}
			catch (InvalidOperationException ex)
			{
				string message = LogActivationException(serviceType, key, ex);

				// Note that the standard implementation of UnityServiceLocator should throw this exception, but
				// does not. It just passes through the container-specific exception.
				throw new ActivationException(message, ex);
			}
			catch (Exception ex)
			{
				LogActivationException(serviceType, key, ex);
				throw;
			}
		}

		private static string LogActivationException(Type serviceType, string key, Exception ex)
		{
			string message = "Failed to resolve type " + serviceType + " with key \"" + key + "\"";
			logger.Error(message, ex);
			return message;
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
			try
			{
				return container.ResolveAll(serviceType);
			}
			catch (InvalidOperationException ex)
			{
				string message = LogActivationException(serviceType, ex);
				throw new ActivationException(message, ex);
			}
		}

		private static string LogActivationException(Type serviceType, Exception ex)
		{
			string message = "Failed to resolve all instances of type " + serviceType;
			logger.Error(message, ex);
			return message;
		}
	}
}