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

namespace Quokka.ServiceLocation
{
	/// <summary>
	/// Interface to basic service container functionality
	/// </summary>
	/// <remarks>
	/// <para>
	/// While the <see cref="IServiceLocator"/> interface provides a mechanism for
	/// locating services in a container-independent way, it does not provide any
	/// way to register services. It also has no mechanism for creating 'child' containers,
	/// which is often useful.
	/// </para>
	/// <para>
	/// This interface can be obtained from the <see cref="IServiceLocator"/> interface,
	/// and provides a way to register services, and create child containers.
	/// </para>
	/// </remarks>
	public interface IServiceContainer : IDisposable
	{
		/// <summary>
		/// The <see cref="IServiceLocator"/> associated with this container
		/// </summary>
		/// <remarks>
		/// The service locator can also be obtained via the <see cref="IServiceProvider.GetService"/> method:
		/// <code>
		/// IServiceLocator serviceLocator = (IServiceLocator) container.GetService(typeof(IServiceLocator));
		/// </code>
		/// </remarks>
		IServiceLocator Locator { get; }

		/// <summary>
		/// Create a child container
		/// </summary>
		/// <returns>
		/// Returns a <see cref="IServiceContainer"/>, which provides access to the created
		/// child service container.
		/// </returns>
		IServiceContainer CreateChildContainer();

		/// <summary>
		/// Register a type mapping with the container
		/// </summary>
		/// <param name="from">
		/// The type that is requested from the container. Often this is an interface type, but it can 
		/// be a concrete type that is the same as <paramref name="to"/>, or a supertype of <paramref name="to"/>.
		/// </param>
		/// <param name="to">
		/// The type that is provided by the container in response to the request.
		/// </param>
		/// <param name="name">
		/// The name associated with the mapping, or <c>null</c> if this is the default mapping.
		/// </param>
		/// <param name="lifecycle">
		/// Specifies whether the container should create a new instance for every request, or always return
		/// the same instance.
		/// </param>
		/// <returns>The <see cref="IServiceContainer"/>.</returns>
		IServiceContainer RegisterType(Type from, Type to, string name, ServiceLifecycle lifecycle);

		/// <summary>
		/// Register an instance of an object with the container.
		/// </summary>
		/// <param name="from">
		/// The type that is requested from the container. Often this is an interface type, but it can be
		/// a concrete type that is the type of <paramref name="instance"/>, or a supertype of the type of
		/// <paramref name="instance"/>.
		/// </param>
		/// <param name="name">
		/// The name associated with the mapping, or <c>null</c> if this is the default mapping.
		/// </param>
		/// <param name="instance">
		/// The object instance that is provided by the container in response to the request.
		/// </param>
		/// <returns>
		/// The <see cref="IServiceContainer"/>.
		/// </returns>
		IServiceContainer RegisterInstance(Type from, string name, object instance);

		/// <summary>
		/// Determine whether a type mapping is registered in the container without necessarily
		/// creating an instance of the object.
		/// </summary>
		/// <param name="type">Type registered with the container</param>
		/// <returns>Returns <see langword="true"/> if the type can be resolved, <see langword="false"/> otherwise.</returns>
		/// <remarks>
		/// This method is used by the framework to register default implementations for framework interfaces.
		/// </remarks>
		bool IsTypeRegistered(Type type);
	}
}