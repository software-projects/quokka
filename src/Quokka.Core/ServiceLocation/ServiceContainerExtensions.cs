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
using System.Reflection;
using Castle.Core.Logging;
using Quokka.Diagnostics;

namespace Quokka.ServiceLocation
{
	public static class ServiceContainerExtensions
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();

		#region RegisterType overloads

		public static IServiceContainer RegisterType(this IServiceContainer container, Type type, string name,
		                                             ServiceLifecycle lifecycle)
		{
			return container.RegisterType(type, null, name, lifecycle);
		}


		public static IServiceContainer RegisterType(this IServiceContainer container, Type from, Type to,
		                                             ServiceLifecycle lifecycle)
		{
			return container.RegisterType(from, to, null, lifecycle);
		}


		public static IServiceContainer RegisterType(this IServiceContainer container, Type type, ServiceLifecycle lifecycle)
		{
			return container.RegisterType(type, null, null, lifecycle);
		}


		public static IServiceContainer RegisterType<TFrom, TTo>(this IServiceContainer container, string name,
		                                                         ServiceLifecycle lifecycle)
			where TTo : TFrom
			where TFrom : class
		{
			return container.RegisterType(typeof (TFrom), typeof (TTo), name, lifecycle);
		}


		public static IServiceContainer RegisterType<T>(this IServiceContainer container, string name,
		                                                ServiceLifecycle lifecycle)
			where T : class
		{
			return container.RegisterType(typeof (T), null, name, lifecycle);
		}


		public static IServiceContainer RegisterType<TFrom, TTo>(this IServiceContainer container, ServiceLifecycle lifecycle)
			where TTo : TFrom
			where TFrom : class
		{
			return container.RegisterType(typeof (TFrom), typeof (TTo), null, lifecycle);
		}


		public static IServiceContainer RegisterType<T>(this IServiceContainer container, ServiceLifecycle lifecycle)
			where T : class
		{
			return container.RegisterType(typeof (T), null, null, lifecycle);
		}

		#endregion

		#region RegisterInstance overloads

		public static IServiceContainer RegisterInstance(this IServiceContainer container, Type type, object instance)
		{
			Verify.ArgumentNotNull(container, "container");
			Verify.ArgumentNotNull(instance, "instance");
			Verify.ArgumentNotNull(type, "type");
			return container.RegisterInstance(type, null, instance);
		}

		public static IServiceContainer RegisterInstance<T>(this IServiceContainer container, string name, T instance)
			where T : class
		{
			Verify.ArgumentNotNull(container, "container");
			Verify.ArgumentNotNull(instance, "instance");
			return container.RegisterInstance(typeof (T), name, instance);
		}

		public static IServiceContainer RegisterInstance<T>(this IServiceContainer container, T instance)
			where T : class
		{
			Verify.ArgumentNotNull(container, "container");
			Verify.ArgumentNotNull(instance, "instance");
			return container.RegisterInstance(typeof (T), null, instance);
		}

		#endregion

		#region IsTypeRegistered overloads

		public static bool IsTypeRegistered<T>(this IServiceContainer container)
		{
			Verify.ArgumentNotNull(container, "container");
			return container.IsTypeRegistered(typeof (T));
		}

		#endregion

		/// <summary>
		/// Register all classes in an assembly that have a [RegisterType] attribute
		/// </summary>
		/// <param name="container">The service container</param>
		/// <param name="assembly">Assembly for registering types</param>
		/// <returns>The service container</returns>
		/// <remarks>
		/// This method is convenient for registering all services in an assembly. The service classes
		/// should have an associated <see cref="RegisterTypeAttribute"/>, or one of its derived attribute classes
		/// (eg <see cref="PerRequestAttribute"/>, <see cref="SingletonAttribute"/>).
		/// </remarks>
		public static IServiceContainer RegisterTypesInAssembly(this IServiceContainer container, Assembly assembly)
		{
			Verify.ArgumentNotNull(container, "container");

			if (assembly != null)
			{
				// Could do this using Linq I know. This way is slightly quicker and not that much harder to understand.
				foreach (Type type in assembly.GetTypes())
				{
					if (!type.IsClass)
					{
						// only interested in the classes
						continue;
					}

					foreach (RegisterTypeAttribute attribute in type.GetCustomAttributes(typeof (RegisterTypeAttribute), false))
					{
						Type fromType = attribute.Type;
						Type toType = type;
						string name = attribute.Name;
						ServiceLifecycle lifecycle = attribute.Lifecycle;
						try
						{
							container.RegisterType(fromType, toType, name, lifecycle);
						}
						catch (Exception ex)
						{
							string message = String.Format("Cannot register type {0} from attribute: {1}", type.FullName, ex.Message);
							Log.Error(message, ex);
							throw;
						}
					}
				}
			}

			return container;
		}
	}
}