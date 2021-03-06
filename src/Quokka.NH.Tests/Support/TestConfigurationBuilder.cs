﻿#region License

// Copyright 2004-2012 John Jeffery
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
using System.Data.Common;
using System.Reflection;
using NHibernate;
using NHibernate.Cfg;
using NHibernate.Connection;
using NHibernate.Dialect;
using NHibernate.Driver;
using NHibernate.Engine;
using NHibernate.Mapping.ByCode;
using NHibernate.Tool.hbm2ddl;
using Quokka.NH.Interfaces;
using Quokka.NH.Startup;
using Environment = NHibernate.Cfg.Environment;

namespace Quokka.NH.Tests.Support
{
	public class TestConfigurationBuilder : IConfigurationBuilder, ISessionFactoryContributor, IDisposable
	{
		public const string Alias = "default";

		public TestConfigurationBuilder()
		{
			CreateDatabaseSchema = true;

			// Attempt to get the test database connection string from the environment.
			// If not available use a default connection string. The teamcity agents
			// have this environment variable set if necessary.
			ConnectionString = System.Environment.GetEnvironmentVariable("QUOKKA_TEST_DATABASE");
			if (string.IsNullOrEmpty(ConnectionString))
			{
				ConnectionString = "Data Source=.;Initial Catalog=QuokkaTest;Integrated Security=SSPI";
			}

		}

		public string ConnectionString { get; set; }
		public bool CreateDatabaseSchema { get; set; }
		public bool DatabaseSchemaCreated { get; private set; }
		public Configuration Configuration { get; private set; }
		public ISessionFactory SessionFactory { get; private set; }

		public bool CanBuildConfiguration(string alias)
		{
			return alias == Alias || alias == null;
		}

		public Configuration BuildConfiguration(string alias)
		{
			var cfg = new Configuration();
			cfg.SetProperty(Environment.ConnectionProvider, typeof (DriverConnectionProvider).AssemblyQualifiedName);
			cfg.SetProperty(Environment.Dialect, typeof (MsSql2005Dialect).AssemblyQualifiedName);
			cfg.SetProperty(Environment.ConnectionDriver, typeof(SqlClientDriver).AssemblyQualifiedName);
			cfg.SetProperty(Environment.ConnectionString, ConnectionString);
			cfg.SetProperty(Environment.CurrentSessionContextClass, "thread_static");

			var modelMapper = new ModelMapper();
			modelMapper.AddMappings(Assembly.GetExecutingAssembly().GetExportedTypes());
			var mapping = modelMapper.CompileMappingForAllExplicitlyAddedEntities();
			cfg.AddMapping(mapping);

			return cfg;
		}

		public void Contribute(string alias, ISessionFactory factory, Configuration cfg)
		{
			Configuration = cfg;
			SessionFactory = factory;
			if (CreateDatabaseSchema)
			{
				using (var session = factory.OpenSession())
				{
					var dialect = Dialect.GetDialect(cfg.Properties);

					var metadata = new DatabaseMetadata((DbConnection) session.Connection, dialect);

					string[] lines = cfg.GenerateSchemaUpdateScript(dialect, metadata);

					foreach (var line in lines)
					{
						var cmd = session.Connection.CreateCommand();
						cmd.CommandText = line;
						cmd.ExecuteNonQuery();
					}
				}

				DatabaseSchemaCreated = true;
			}
		}

		public void Dispose()
		{
			if (DatabaseSchemaCreated)
			{
				var sessionFactoryImplementor = (ISessionFactoryImplementor)SessionFactory;

				string[] lines = Configuration.GenerateDropSchemaScript(sessionFactoryImplementor.Dialect);

				using (var session = SessionFactory.OpenSession())
				{
					foreach (var line in lines)
					{
						var cmd = session.Connection.CreateCommand();
						cmd.CommandText = line;
						cmd.ExecuteNonQuery();
					}
				}
			}
		}
	}
}
