#region License

//
//  Notice: Some of the code in this file may have been adapted from code
//  in the Castle Project.
//
//  Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

using NHibernate.Cfg;

namespace Quokka.NH.Startup
{
	/// <summary>
	/// Registers an object which optionally contributes to the NHibernate <see cref="Configuration"/>,
	/// which has just been created by an <see cref="IConfigurationBuilder"/>.
	/// </summary>
	/// <remarks>
	/// The NHibernate facility creates NHibernate configurations by resolving all components that
	/// implement the <see cref="IConfigurationBuilder"/> interface. After each <see cref="Configuration"/>
	/// object is created, the NHibernate facility calls each component registered to implement this
	/// interface. This provides an opportunity to add additional class mappings, modify the configuration,
	/// add interceptors and more.
	/// </remarks>
	public interface IConfigurationContributor
	{
		/// <summary>
		/// Contribute to the NHibernate <see cref="Configuration"/>.
		/// </summary>
		/// <param name="alias">
		/// The alias of the database configuration being created. This value
		/// comes from the <see cref="IConfigurationBuilder.Alias"/> property.
		/// </param>
		/// <param name="isDefault">Is this the default configuration.</param>
		/// <param name="configuration">NHibernate <see cref="Configuration"/> object.</param>
		void Contribute(string alias, bool isDefault, Configuration configuration);
	}
}