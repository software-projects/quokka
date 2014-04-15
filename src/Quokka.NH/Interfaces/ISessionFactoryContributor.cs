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
using NHibernate.Cfg;

namespace Quokka.NH.Interfaces
{
	/// <summary>
	/// Registers an object which optionally contributes to the NHibernate <see cref="ISessionFactory"/>,
	/// which has just been created by an <see cref="ISessionFactoryResolver"/>.
	/// </summary>
	/// <remarks>
	/// The NHibernate facility creates NHibernate session factories as required. After a session factory
	/// has been created, the NHibernate facility calls each component registered to implement this
	/// interface. This provides an opportunity to update the database schema, populate the database,
	/// manage the 2nd level cache and more.
	/// </remarks>
	public interface ISessionFactoryContributor
	{
		void Contribute(string alias, ISessionFactory sessionFactory, Configuration configuration);
	}
}
