#region License

//
//  Copyright 2004-2012 John Jeffery
//  
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  
//      http://www.apache.org/licenses/LICENSE-2.0
//  
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// 

#endregion

using NHibernate;
using NHibernate.Cfg;

namespace Quokka.NH.Startup
{
	/// <summary>
	/// Interface implemented by classes that can create an NHibernate configuration for a database.
	/// </summary>
	/// <remarks>
	/// Register at least one <see cref="IConfigurationBuilder"/> with the container.
	/// </remarks>
	public interface IConfigurationBuilder
	{
		/// <summary>
		/// Identifies whether this builder can create a <see cref="Configuration"/> for the
		/// specified database alias. A single configuration builder can be capable of building
		/// configurations for any number of different aliases.
		/// </summary>
		/// <param name="alias">The database alias. May be <c>null</c> if no default alias has been specified.</param>
		/// <returns>
		/// Returns <c>true</c> if this builder can create a <see cref="Configuration"/>,
		/// <c>false</c> otherwise.
		/// </returns>
		bool CanBuildConfiguration(string alias);

		/// <summary>
		/// 	Build an NHibernate <see cref="Configuration"/> for this database.
		/// </summary>
		/// <param name="alias">
		///		The database alias to create a <see cref="Configuration"/> for.
		/// </param>
		/// <returns>
		///		A non null <see cref="Configuration"/> instance that can
		/// 	be used to create an <see cref="ISessionFactory"/>, or to further 
		///		configure NHibernate.
		/// </returns>
		Configuration BuildConfiguration(string alias);
	}
}