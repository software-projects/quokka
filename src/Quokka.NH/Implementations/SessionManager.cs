#region License

//
// Copyright 2004-2012 John Jeffery <john@jeffery.id.au>
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

using NHibernate;
using NHibernate.Context;
using Quokka.Diagnostics;
using Quokka.NH.Interfaces;
using Quokka.NH.Startup;

namespace Quokka.NH.Implementations
{
	public class SessionManager : ISessionManager
	{
		private readonly ISessionFactoryResolver _sessionFactoryResolver;

		public SessionManager(ISessionFactoryResolver sessionFactoryResolver)
		{
			_sessionFactoryResolver = Verify.ArgumentNotNull(sessionFactoryResolver, "sessionFactoryResolver");
		}

		public ISession OpenSession(string alias = null)
		{
			alias = NormaliseAlias(alias);
			var canClose = false;
			var unbindOnClose = false;
			var sessionFactory = _sessionFactoryResolver.GetSessionFactory(alias);
			ISession session;

			// First rule is that if there is a session in the current context, use it.
			if (CurrentSessionContext.HasBind(sessionFactory))
			{
				session = sessionFactory.GetCurrentSession();
			}
			else
			{
				// Second rule is try to find from dervived class's storage mechanism (if any exists).
				session = FindCompatibleSession(alias, sessionFactory);
				if (session != null)
				{
					// want to unbind when the session delegate is closed,
					// but do not close the session as it is pre-existing
					unbindOnClose = true;
					CurrentSessionContext.Bind(session);
				}
				else
				{
					session = CreateNewSession(alias, sessionFactory);
					CurrentSessionContext.Bind(session);
					canClose = true;
					unbindOnClose = true;
				}
			}

			var sessionDelegate = new SessionDelegate(canClose, session);
			if (unbindOnClose)
			{
				sessionDelegate.Closed += (o, e) => CurrentSessionContext.Unbind(sessionFactory);
			}

			return sessionDelegate;
		}

		/// <summary>
		/// Opens a stateless session. No fancy sharing performed here. Each call returns a new
		/// <see cref="IStatelessSession"/>.
		/// </summary>
		public IStatelessSession OpenStatelessSession(string alias = null)
		{
			alias = NormaliseAlias(alias);
			var sessionFactory = _sessionFactoryResolver.GetSessionFactory(alias);
			return sessionFactory.OpenStatelessSession();
		}

		/// <summary>
		/// Provides a mechanism for a derived class to find sessions from a separate context.
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="sessionFactory"></param>
		/// <returns>
		/// Returns a compatible <see cref="ISession"/> whose lifetime is controlled externally.
		/// </returns>
		protected virtual ISession FindCompatibleSession(string alias, ISessionFactory sessionFactory)
		{
			return null;
		}

		/// <summary>
		/// Creates a new <see cref="ISession"/> from the <see cref="ISessionFactory"/>. Allows
		/// a derived class to 
		/// </summary>
		/// <param name="alias"></param>
		/// <param name="sessionFactory"></param>
		/// <returns></returns>
		/// <remarks>
		/// One use for overriding this method is to create sessions with an <see cref="IInterceptor"/>
		/// implementation.
		/// </remarks>
		protected virtual ISession CreateNewSession(string alias, ISessionFactory sessionFactory)
		{
			return sessionFactory.OpenSession();
		}


		private string NormaliseAlias(string alias)
		{
			return alias ?? _sessionFactoryResolver.DefaultAlias;
		}
	}
}
