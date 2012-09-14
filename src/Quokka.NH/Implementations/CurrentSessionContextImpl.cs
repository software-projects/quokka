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