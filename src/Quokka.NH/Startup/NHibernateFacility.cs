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

using Castle.Core.Logging;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using NHibernate.Context;
using Quokka.NH.Implementations;
using Quokka.NH.Interfaces;
using ILoggerFactory = Castle.Core.Logging.ILoggerFactory;

namespace Quokka.NH.Startup
{
	public class NHibernateFacility : AbstractFacility
	{
		private ILogger _logger;
		private ILoggerFactory _loggerFactory = new NullLogFactory();

		/// <summary>
		/// The alias to use when opening a session and no alias is supplied.
		/// </summary>
		public string DefaultAlias { get; set; }

		public NHibernateFacility()
		{
			DefaultAlias = "default";
		}

		protected override void Init()
		{
			// Create a logger if there is a logger factory.
			if (Kernel.HasComponent(typeof(ILoggerFactory)))
			{
				_loggerFactory = Kernel.Resolve<ILoggerFactory>();
			}
			_logger = _loggerFactory.Create(typeof(NHibernateFacility));


			_logger.Debug("Initializing NHibernateFacility");

			RegisterDefaultAliasContributor();

			RegisterTransactionalInfoStore();
			RegisterTransactionInterceptor();
			RegisterConfigurationResolver();
			RegisterSessionFactoryResolver();
			RegisterSessionManager();
			RegisterCurrentSessionContext();
			AddContributors();

			_logger.Debug("NHibernateFacility is initialized");
		}

		private void RegisterDefaultAliasContributor()
		{
			// This contributor will populate the DefaultAlias property of any component created that
			// implements the IDefaultAlias interface.
			Kernel.ComponentModelBuilder.AddContributor(new DefaultAliasContributor(GetDefaultAlias));
		}


		private string GetDefaultAlias()
		{
			return DefaultAlias;
		}

		private void RegisterTransactionalInfoStore()
		{
			Kernel.Register(Component.For<ITransactionalInfoStore>()
								.ImplementedBy<TransactionMetaInfoStore>()
								.LifestyleSingleton());
		}

		private void RegisterTransactionInterceptor()
		{
			Kernel.Register(Component.For<NHTransactionInterceptor>().LifestyleTransient());
		}

		private void RegisterConfigurationResolver()
		{
			if (!Kernel.HasComponent(typeof(IConfigurationResolver)))
			{
				Kernel.Register(Component.For<IConfigurationResolver>()
					.ImplementedBy<ConfigurationResolver>()
					.LifestyleSingleton());
			}
		}

		private void RegisterSessionFactoryResolver()
		{
			if (!Kernel.HasComponent(typeof(ISessionFactoryResolver)))
			{
				Kernel.Register(Component.For<ISessionFactoryResolver>()
					.ImplementedBy<SessionFactoryResolver>()
					.LifestyleSingleton());
			}
		}

		private void RegisterSessionManager()
		{
			if (!Kernel.HasComponent(typeof(ISessionManager)))
			{
				Kernel.Register(Component.For<ISessionManager>()
									.ImplementedBy<TaskAwareSessionManager>()
									.LifestyleTransient());
			}
		}

		private void RegisterCurrentSessionContext()
		{
			if (!Kernel.HasComponent(typeof(ICurrentSessionContext)))
			{
				Kernel.Register(Component.For<ICurrentSessionContext>()
				                	.ImplementedBy<CurrentSessionContextImpl>()
									.LifestyleSingleton());
			}
		}

		private void AddContributors()
		{
			Kernel.ComponentModelBuilder.AddContributor(new NHTransactionalComponentInspector());
		}
	}
}
