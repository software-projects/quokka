﻿#region License

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
using Castle.MicroKernel;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Quokka.ServiceLocation;

namespace Quokka.Castle
{
	/// <summary>
	/// Adapts the behavior of the Windsor container to the common
	/// IServiceLocator
	/// </summary>
	public class WindsorServiceLocator : ServiceLocatorImplBase
	{
		private readonly IWindsorContainer _container;

		/// <summary>
		/// Initializes a new instance of the <see cref="WindsorServiceLocator"/> class.
		/// </summary>
		/// <param name="container">The container.</param>
		public WindsorServiceLocator(IWindsorContainer container)
		{
			this._container = container;
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
				// provides a handy way to get access to the underlying container
				if (serviceType == typeof(IWindsorContainer) && key == null)
				{
					return _container;
				}

				// Currently, Windsor Container does not support the functionality of creating
				// concrete components without registering them first. Quokka (and Prism, for that matter)
				// rely on being able to do this.
				//
				// BTW I got the idea from the following post at stack overflow:
				// http://stackoverflow.com/questions/447193/resolving-classes-without-registering-them-using-castle-windsor
				if (serviceType.IsClass && !_container.Kernel.HasComponent(serviceType))
				{
					_container.Kernel.Register(Component.For(serviceType).LifeStyle.Transient);
				}

				if (key != null)
				{
					return _container.Resolve(key, serviceType);
				}
				return _container.Resolve(serviceType);
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
			return (object[])_container.ResolveAll(serviceType);
		}

		protected override void DoRelease(object instance)
		{
			_container.Release(instance);
		}
	}
}