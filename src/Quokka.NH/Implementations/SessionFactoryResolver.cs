#region License

//  Notice: Some of the code in this file may have been adapted from code
//  in the Castle Project.
//
// Copyright 2004-2012 Castle Project - http://www.castleproject.org/
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

using Castle.Core.Internal;
using Castle.MicroKernel;
using NHibernate;
using NHibernate.Cfg;
using Quokka.Diagnostics;
using Quokka.NH.Interfaces;
using Quokka.NH.Startup;

namespace Quokka.NH.Implementations
{
	// TODO: needs to be threadsafe
	public class SessionFactoryResolver : ISessionFactoryResolver
	{
		private readonly AliasDictionary<ISessionFactory> _sessionFactories = new AliasDictionary<ISessionFactory>();
		private readonly IConfigurationResolver _configurationResolver;
		private readonly IKernel _kernel;
		private readonly Lock _lock = Lock.Create();

		public SessionFactoryResolver(IKernel kernel, IConfigurationResolver configurationResolver)
		{
			_kernel = Verify.ArgumentNotNull(kernel, "kernel");
			_configurationResolver = Verify.ArgumentNotNull(configurationResolver, "configurationResolver");
		}

		public ISessionFactory GetSessionFactory(string alias)
		{
			alias = alias ?? DefaultAlias;

			using (_lock.ForReading())
			{
				ISessionFactory sessionFactory = _sessionFactories.Find(alias);
				if (sessionFactory != null)
				{
					// already created
					return sessionFactory;
				}
			}

			// TODO: this will hold up two threads trying to create session factories for two different
			// database aliases, but the code is simple. Need to improve later.

			using (_lock.ForWriting())
			{
				// look again while we have the write lock, just to check that another thread
				// has not already created the session factory.
				ISessionFactory sessionFactory = _sessionFactories.Find(alias);
				if (sessionFactory != null)
				{
					// already created
					return sessionFactory;
				}

				// this will throw an exception if unsuccessful
				var configuration = _configurationResolver.GetConfiguration(alias);
				sessionFactory = configuration.BuildSessionFactory();
				CallContributors(alias, sessionFactory, configuration);
				_sessionFactories.Save(alias, sessionFactory);

				return sessionFactory;
			}
		}

		public bool IsAliasDefined(string alias)
		{
			using (_lock.ForReading())
			{
				if (_sessionFactories.Find(alias) != null)
				{
					return true;
				}
			}
			return _configurationResolver.IsAliasDefined(alias);
		}

		// auto-populated by the NHibernate facility
		public string DefaultAlias { get; set; }

		/// <summary>
		/// Once a session factory has been created, call all registered contributors
		/// </summary>
		private void CallContributors(string alias, ISessionFactory sessionFactory, Configuration configuration)
		{
			var contributors = _kernel.ResolveAll<ISessionFactoryContributor>();
			try
			{
				foreach (var contributor in contributors)
				{
					contributor.Contribute(alias, sessionFactory, configuration);
				}
			}
			finally
			{
				foreach (var contributor in contributors)
				{
					_kernel.ReleaseComponent(contributor);
				}
			}
		}
	}
}
