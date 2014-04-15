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
using Castle.Core;
using NHibernate;
using NHibernate.Context;
using Quokka.Diagnostics;
using Quokka.NH.Interfaces;

namespace Quokka.NH.Implementations
{
	/// <summary>
	/// Implements the NHibernate <see cref="ICurrentSessionContext"/> interface.
	/// </summary>
	/// <remarks>
	/// <para>Finds the current session context for the default session factory.</para>
	/// <para>Class suffix "Impl" so as not to confuse with the NHibernate <see cref="CurrentSessionContext"/> class.</para>
	/// </remarks>
	[Singleton]
	public class CurrentSessionContextImpl : ICurrentSessionContext
	{
		private readonly ISessionFactoryResolver _sessionFactoryResolver;

		public CurrentSessionContextImpl(ISessionFactoryResolver sessionFactoryResolver)
		{
			_sessionFactoryResolver = Verify.ArgumentNotNull(sessionFactoryResolver, "sessionFactoryResolver");
		}

		public ISession CurrentSession()
		{
			var sessionFactory = _sessionFactoryResolver.GetSessionFactory(_sessionFactoryResolver.DefaultAlias);
			return sessionFactory.GetCurrentSession();
		}
	}
}