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

using System;
using NHibernate;

namespace Quokka.NH
{
	/// <summary>
	/// Provides a bridge to NHibernate allowing the implementation
	/// to cache created sessions as appropriate for the application.
	/// </summary>
	public interface ISessionManager
	{
		/// <summary>
		/// Returns a valid opened and connected <see cref="ISession"/> instance
		/// for the given connection alias.
		/// </summary>
		/// <param name="alias">
		/// Connection alias. If <c>null</c>, then the default alias is used.
		/// </param>
		ISession OpenSession(String alias = null);

		/// <summary>
		/// Returns a valid opened and connected <see cref="IStatelessSession"/> instance
		/// for the given connection alias.
		/// </summary>
		/// <param name="alias"></param>
		/// <returns></returns>
		IStatelessSession OpenStatelessSession(String alias = null);
	}
}