#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;

namespace Quokka.ServiceLocation
{
	/// <summary>
	/// This class is a helper that provides a default implementation
	/// for most of the methods of <see cref="IServiceLocator"/>.
	/// </summary>
	/// <remarks>
	/// Adapted from Microsoft Common Service Locator: http://commonservicelocator.codeplex.com/
	/// </remarks>
	public abstract class ServiceLocatorImplBase : IServiceLocator, IServiceProvider
	{
		/// <summary>
		/// Implementation of <see cref="M:System.IServiceProvider.GetService(System.Type)"/>.
		/// </summary>
		/// <param name="serviceType">
		/// The requested service.</param><exception cref="ActivationException">if there is an error in 
		/// resolving the service instance.</exception>
		/// <returns>
		/// The requested object.
		/// </returns>
		public virtual object GetService(Type serviceType)
		{
			return GetInstance(serviceType, (string) null);
		}

		/// <summary>
		/// Get an instance of the given <paramref name="serviceType"/>.
		/// </summary>
		/// <param name="serviceType">
		/// Type of object requested.
		/// </param>
		/// <exception cref="ActivationException">
		/// if there is an error resolving
		/// the service instance.</exception>
		/// <returns>
		/// The requested service instance.
		/// </returns>
		public virtual object GetInstance(Type serviceType)
		{
			return GetInstance(serviceType, null);
		}

		/// <summary>
		/// Get an instance of the given named <paramref name="serviceType"/>.
		/// </summary>
		/// <param name="serviceType">
		/// Type of object requested.</param>
		/// <param name="key">
		/// Name the object was registered with.</param>
		/// <exception cref="ActivationException">
		/// if there is an error resolving the service instance.
		/// </exception>
		/// <returns>
		/// The requested service instance.
		/// </returns>
		public virtual object GetInstance(Type serviceType, string key)
		{
			try
			{
				return DoGetInstance(serviceType, key);
			}
			catch (Exception ex)
			{
				throw new ActivationException(FormatActivationExceptionMessage(ex, serviceType, key), ex);
			}
		}

		/// <summary>
		/// Get all instances of the given <paramref name="serviceType"/> currently
		///             registered in the container.
		/// 
		/// </summary>
		/// <param name="serviceType">Type of object requested.</param><exception cref="T:Microsoft.Practices.ServiceLocation.ActivationException">if there is are errors resolving
		///             the service instance.</exception>
		/// <returns>
		/// A sequence of instances of the requested <paramref name="serviceType"/>.
		/// </returns>
		public virtual IEnumerable<object> GetAllInstances(Type serviceType)
		{
			try
			{
				return DoGetAllInstances(serviceType);
			}
			catch (Exception ex)
			{
				throw new ActivationException(FormatActivateAllExceptionMessage(ex, serviceType), ex);
			}
		}


		public virtual TService GetInstance<TService>()
		{
			return (TService) GetInstance(typeof (TService), (string) null);
		}

		public virtual TService GetInstance<TService>(string key)
		{
			return (TService) GetInstance(typeof (TService), key);
		}

		public virtual IEnumerable<TService> GetAllInstances<TService>()
		{
			using (IEnumerator<object> enumerator = GetAllInstances(typeof (TService)).GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					object item = enumerator.Current;
					yield return (TService) item;
				}
			}
		}

		public virtual void Release(object instance)
		{
			DoRelease(instance);
		}

		protected abstract object DoGetInstance(Type serviceType, string key);
		protected abstract IEnumerable<object> DoGetAllInstances(Type serviceType);
		protected abstract void DoRelease(object instance);

		protected virtual string FormatActivationExceptionMessage(Exception actualException, Type serviceType, string key)
		{
			return string.Format("Activation error occurred while trying to get instance of type {0}, key \"{1}\"",
			                     serviceType, key);
		}

		protected virtual string FormatActivateAllExceptionMessage(Exception actualException, Type serviceType)
		{
			return string.Format("Activation error occurred while trying to get all instances of type {0}", serviceType);
		}
	}
}