using System;
using System.Collections.Generic;

namespace Quokka.ServiceLocation
{
	/// <summary>
	/// This is similar to the Microsoft service locator, with the exception
	/// that it has a Release method (for castle).
	/// </summary>
	public interface IServiceLocator : IServiceProvider
	{
		object GetInstance(Type serviceType, string key = null);
		IEnumerable<object> GetAllInstances(Type serviceType);
		TService GetInstance<TService>(string key = null);
		IEnumerable<TService> GetAllInstances<TService>();
		void Release(object instance);
	}
}
