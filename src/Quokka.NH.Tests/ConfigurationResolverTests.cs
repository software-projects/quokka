#region License

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
using System.Collections.Generic;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate;
using NHibernate.Cfg;
using NUnit.Framework;
using Quokka.NH.Implementations;
using Quokka.NH.Startup;
using Quokka.NH.Tests.Support;

// ReSharper disable InconsistentNaming
namespace Quokka.NH.Tests
{
	[TestFixture]
	public class ConfigurationResolverTests
	{
		[Test]
		public void Installers_defined_before_ConfigurationResolver_are_processed()
		{
			var container = new WindsorContainer();

			// Register installer prior to registering 
			container.Register(
				Component.For<IConfigurationBuilder>()
					.ImplementedBy<DefaultInstaller>()
				);

			var configurationResolver = new ConfigurationResolver(container.Kernel);
			Assert.IsTrue(configurationResolver.IsAliasDefined("default-installer"));
		}

		[Test]
		public void Installers_defined_after_ConfigurationResolver_are_processed()
		{
			var container = new WindsorContainer();
			var configurationResolver = new ConfigurationResolver(container.Kernel);

			// Register installer prior to registering 
			container.Register(
				Component.For<IConfigurationBuilder>()
					.ImplementedBy<DefaultInstaller>()
				);

			Assert.IsTrue(configurationResolver.IsAliasDefined("default-installer"));
		}

		[Test]
		public void Installers_with_dependencies_are_processed_when_possible()
		{
			var container = new WindsorContainer();
			var configurationResolver = new ConfigurationResolver(container.Kernel);

			// Register installer prior to registering 
			container.Register(
				Component.For<IConfigurationBuilder>()
					.ImplementedBy<InstallerWithDependency>()
				);

			Assert.IsFalse(configurationResolver.IsAliasDefined("installer-with-dependency"));

			container.Register(Component.For<DependencyClass>());

			Assert.IsTrue(configurationResolver.IsAliasDefined("installer-with-dependency"));
		}

		[Test]
		public void No_default_installer()
		{
			var container = new WindsorContainer();
			var configurationResolver = new ConfigurationResolver(container.Kernel);

			Assert.IsNull(configurationResolver.DefaultAlias);

			container.Register(
				Component.For<IConfigurationBuilder>()
					.ImplementedBy<NonDefaultInstaller>()
				);

			Assert.IsNull(configurationResolver.DefaultAlias);
			Assert.Throws<NHibernateFacilityException>(() => configurationResolver.GetConfiguration(null));
		}

		[Test]
		public void Unknown_alias()
		{
			var container = new WindsorContainer();
			var configurationResolver = new ConfigurationResolver(container.Kernel);

			container.Register(
				Component.For<IConfigurationBuilder>()
					.ImplementedBy<NonDefaultInstaller>()
				);

			Assert.Throws<NHibernateFacilityException>(() => configurationResolver.GetConfiguration("some-random-alias"));
		}

		[Test]
		public void Default_alias_used_for_null_alias()
		{
			using (var container = new WindsorContainer())
			{
				var configurationResolver = new ConfigurationResolver(container.Kernel) {DefaultAlias = "testdb"};

				// Register installer prior to registering 
				container.Register(
					Component.For<IConfigurationBuilder>()
						.ImplementedBy<TestConfigurationBuilder>(),
					Component.For<IConfigurationBuilder>()
						.ImplementedBy<NonDefaultInstaller>(),
					Component.For<IConfigurationSerializer>()
						.ImplementedBy<TestSerializer>()
					);

				var testdbConfiguration = configurationResolver.GetConfiguration("testdb");
				var defaultConfiguration = configurationResolver.GetConfiguration(null);

				Assert.AreSame(testdbConfiguration, defaultConfiguration);
			}
		}

		// used for remembering configurations -- otherwise a new configuration is created each time
		private class TestSerializer : IConfigurationSerializer
		{
			private readonly Dictionary<string, Configuration> _configurations = new Dictionary<string, Configuration>();

			public bool CanSerialize(string alias)
			{
				return true;
			}

			public Configuration Deserialize(string alias)
			{
				Assert.IsNotNull(alias);
				Configuration configuration;
				_configurations.TryGetValue(alias, out configuration);
				return configuration;
			}

			public void Serialize(string alias, Configuration configuration)
			{
				Assert.IsNotNull(alias);
				Assert.IsNotNull(configuration);
				_configurations[alias] = configuration;
			}
		}


		[Test]
		public void Throws_exception_if_null_argument_in_constructor()
		{
			try
			{
				new ConfigurationResolver(null);
				Assert.Fail("Expected exception");
			}
			catch (ArgumentNullException) { }
		}

		#region class DefaultInstaller

		// ReSharper disable ClassNeverInstantiated.Local
		private class DefaultInstaller : IConfigurationBuilder
		{
			public string Alias
			{
				get { return "default-installer"; }
			}

			public bool CanBuildConfiguration(string alias)
			{
				return alias == Alias;
			}

			public Configuration BuildConfiguration(string alias)
			{
				throw new NotImplementedException();
			}

			public void SessionFactoryCreated(string alias, Configuration configuration, ISessionFactory sessionFactory)
			{
				
			}

		}

		#endregion

		#region class AnotherDefaultInstaller

		// ReSharper disable ClassNeverInstantiated.Local
		private class AnotherDefaultInstaller : IConfigurationBuilder
		{
			public string Alias
			{
				get { return "another-default-installer"; }
			}

			public bool CanBuildConfiguration(string alias)
			{
				return alias == Alias;
			}

			public Configuration BuildConfiguration(string alias)
			{
				throw new NotImplementedException();
			}

			public void SessionFactoryCreated(string alias, Configuration configuration, ISessionFactory sessionFactory)
			{

			}

		}

		#endregion

		#region class NonDefaultInstaller

		// ReSharper disable ClassNeverInstantiated.Local
		private class NonDefaultInstaller : IConfigurationBuilder
		{
			public string Alias
			{
				get { return "non-default-installer"; }
			}

			public bool CanBuildConfiguration(string alias)
			{
				return alias == Alias;
			}

			public Configuration BuildConfiguration(string alias)
			{
				throw new NotImplementedException();
			}

			public void SessionFactoryCreated(string alias, Configuration configuration, ISessionFactory sessionFactory)
			{

			}
		}

		#endregion

		#region class InstallerWithDependency

		public class InstallerWithDependency : IConfigurationBuilder
		{
			// ReSharper disable UnusedParameter.Local
			public InstallerWithDependency(DependencyClass dependencyClass)
			{

			}
			// ReSharper restore UnusedParameter.Local

			public string Alias
			{
				get { return "installer-with-dependency"; }
			}

			public bool CanBuildConfiguration(string alias)
			{
				return alias == Alias;
			}

			public Configuration BuildConfiguration(string alias)
			{
				throw new NotImplementedException();
			}

			public void SessionFactoryCreated(string alias, Configuration configuration, ISessionFactory sessionFactory)
			{

			}
		}

		#endregion

		#region class DependencyClass

		public class DependencyClass { }

		#endregion
	}
}
