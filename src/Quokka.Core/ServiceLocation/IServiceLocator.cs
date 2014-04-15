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
