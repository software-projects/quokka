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
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NHibernate.Cfg;
using NUnit.Framework;
using Quokka.NH.Implementations;
using Quokka.NH.Interfaces;
using Quokka.NH.Startup;

// ReSharper disable InconsistentNaming
namespace Quokka.NH.Tests
{
	[TestFixture]
	public class SessionFactoryResolverTests
	{
		[Test]
		public void Default_alias_comes_from_facility()
		{
			var container = new WindsorContainer();
			container.AddFacility<NHibernateFacility>(f => f.DefaultAlias = "OFO-649");
			var sessionFactoryResolver = container.Resolve<ISessionFactoryResolver>();
			Assert.AreEqual("OFO-649", sessionFactoryResolver.DefaultAlias);
		}

		[Test]
		public void Installers_defined_after_SessionFactoryResolver_are_processed()
		{
			var container = new WindsorContainer();
			container.AddFacility<NHibernateFacility>();
			var sessionFactoryResolver = container.Resolve<ISessionFactoryResolver>();

			// Register installer prior to registering 
			container.Register(Component.For<IConfigurationBuilder>().ImplementedBy<DefaultInstaller>());

			Assert.IsTrue(sessionFactoryResolver.IsAliasDefined("default-installer"));
		}

		[Test]
		public void Throws_exception_if_null_argument_in_constructor()
		{
			Assert.Throws<ArgumentNullException>(() => new SessionFactoryResolver(null, null));
		}

		#region class DefaultInstaller

		// ReSharper disable ClassNeverInstantiated.Local
		private class DefaultInstaller : IConfigurationBuilder
		{
			public const string Alias = "default-installer";

			public bool CanBuildConfiguration(string alias)
			{
				return alias == Alias;
			}

			public Configuration BuildConfiguration(string alias)
			{
				throw new NotImplementedException();
			}
		}

		#endregion
	}
}
