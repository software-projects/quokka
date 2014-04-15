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
using NHibernate;

namespace Quokka.NH.Interfaces
{
	/// <summary>
	/// Implement this interface to save instances of <see cref="ISessionFactory"/> for 
	/// later use. Useful for speeding up automated tests, probably not useful in production.
	/// </summary>
	/// <remarks>
	/// Register a component implementing this interface in the Windsor container during automated
	/// testing. The first time a session factory is needed it will be created as normal and then
	/// saved via this interface. The next time a session factory is needed, it can be retrieved via
	/// this interface, saving a lot of time in test fixture setups.
	/// </remarks>
	public interface ISessionFactoryCache
	{
		/// <summary>
		/// Try to find the <see cref="ISessionFactory"/> for the given alias.
		/// </summary>
		ISessionFactory Find(string alias);

		/// <summary>
		/// Save the <see cref="ISessionFactory"/> associated with the given alias for later use.
		/// </summary>
		void Save(string alias, ISessionFactory sessionFactory);
	}
}