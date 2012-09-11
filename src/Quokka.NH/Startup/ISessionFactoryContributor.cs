using System;
using NHibernate;
using NHibernate.Cfg;
using Quokka.NH.Interfaces;

namespace Quokka.NH.Startup
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
