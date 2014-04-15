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
using System.Collections.Generic;
using System.Linq;
using Castle.MicroKernel;
using Castle.MicroKernel.Context;
using NHibernate.Cfg;
using Quokka.Diagnostics;
using Quokka.NH.Interfaces;
using Quokka.NH.Startup;

namespace Quokka.NH.Implementations
{
	public class ConfigurationResolver : IConfigurationResolver
	{
		public IKernel Kernel { get; private set; }

		public ConfigurationResolver(IKernel kernel)
		{
			Kernel = Verify.ArgumentNotNull(kernel, "kernel");
		}

		/// <summary>
		/// The default database alias as specified in the NHibernate facility configuration. This property
		/// is automatically set by the NHibernate facility when this component is created by the container.
		/// </summary>
		public string DefaultAlias { get; set; }

		private string NormalizeAlias(string alias)
		{
			return alias ?? DefaultAlias;
		}

		public bool IsAliasDefined(string alias)
		{
			alias = NormalizeAlias(alias);
			var result = false;

			var configurationBuilder = FindConfigurationBuilder(alias);
			if (configurationBuilder != null)
			{
				result = true;
				Kernel.ReleaseComponent(configurationBuilder);
			}

			return result;
		}

		public Configuration GetConfiguration(string alias)
		{
			// collection of components that need to be released
			var components = new List<object>();
			Configuration configuration = null;

			try
			{
				alias = NormalizeAlias(alias);

				// let the derived class (if any) attempt to de-serialize the configuration
				// not sure if this is useful, as it is probably more convenient to register
				// an IConfigurationSerializer component than it is to derive from this class.
				configuration = Find(alias);
				if (configuration != null)
				{
					return configuration;
				}

				// attempt to deserialize the configuration
				IConfigurationSerializer configurationSerializer = null;
				if (alias != null)
				{
					configurationSerializer = FindConfigurationSerializer(alias);
					if (configurationSerializer != null)
					{
						components.Add(configurationSerializer);
						configuration = configurationSerializer.Deserialize(alias);
						if (configuration != null)
						{
							return configuration;
						}
					}
				}

				IConfigurationBuilder configurationBuilder = FindConfigurationBuilder(alias);
				if (configurationBuilder == null)
				{
					var aliasText = alias ?? "<null>";
					string message = string.Format("Cannot create a NHibernate Configuration for alias '{0}'."
					                               + " No IConfigurationBuilder is registered.", aliasText);
					throw new NHibernateFacilityException(message);
				}

				var configurationContributors = Kernel.ResolveAll<IConfigurationContributor>();
				components.AddRange(configurationContributors);

				configurationContributors = Kernel.ResolveAll<IConfigurationContributor>();
				configuration = configurationBuilder.BuildConfiguration(alias);
				foreach (var configurationContributor in configurationContributors)
				{
					configurationContributor.Contribute(alias, configuration);
				}

				// the configuration has been created, so let the serializer serialize it (if it exists)
				if (configurationSerializer != null)
				{
					configurationSerializer.Serialize(alias, configuration);
				}
			}
			finally
			{
				foreach (var component in components)
				{
					Kernel.ReleaseComponent(component);
				}
				components.Clear();
			}

			return configuration;
		}

		// find a configuration serializer that can deserialze a configuration for the alias
		private IConfigurationSerializer FindConfigurationSerializer(string alias)
		{
			// Resolve all possible configuration builders. We go to some effort to
			// not throw an exception if a configuration builder is not ready to
			// be built because of an unconfigured dependency. The thinking here
			// is that database access can start for one database config while the
			// other database configs are still being configured.
			var handlers = Kernel.GetHandlers(typeof(IConfigurationSerializer));
			var configurationSerializers = handlers
				.Select(handler => (IConfigurationSerializer)handler.TryResolve(CreationContext.CreateEmpty()))
				.Where(configurationSerializer => configurationSerializer != null)
				.ToArray();

			IConfigurationSerializer result = null;

			foreach (var configurationSerializer in configurationSerializers)
			{
				if (result == null && configurationSerializer.CanSerialize(alias))
				{
					result = configurationSerializer;
				}
				else
				{
					Kernel.ReleaseComponent(configurationSerializer);
				}
			}

			return result;
		}

		// find a configuration builder that can build the alias
		private IConfigurationBuilder FindConfigurationBuilder(string alias)
		{
			// Resolve all possible configuration builders. We go to some effort to
			// not throw an exception if a configuration builder is not ready to
			// be built because of an unconfigured dependency. The thinking here
			// is that database access can start for one database config while the
			// other database configs are still being configured.
			var handlers = Kernel.GetHandlers(typeof (IConfigurationBuilder));
			var configurationBuilders = handlers
				.Select(handler => (IConfigurationBuilder) handler.TryResolve(CreationContext.CreateEmpty()))
				.Where(configurationBuilder => configurationBuilder != null)
				.ToArray();

			// find the first configuration builder that can build the specified alias
			IConfigurationBuilder result = null;

			foreach (var configurationBuilder in configurationBuilders)
			{
				if (result == null && configurationBuilder.CanBuildConfiguration(alias))
				{
					result = configurationBuilder;
				}
				else
				{
					Kernel.ReleaseComponent(configurationBuilder);
				}
			}


			return result;
		}

		/// <summary>
		/// Derived classes can override this method to provide custom de-serialization
		/// </summary>
		/// <remarks>
		/// Probably more convenient to register an implementation of <see cref="IConfigurationSerializer"/>.
		/// </remarks>
		protected virtual Configuration Find(string alias)
		{
			return null;
		}

		/// <summary>
		/// Derived classes can override this method to provide custom serialization
		/// </summary>
		/// <remarks>
		/// Probably more convenient to register an implementation of <see cref="IConfigurationSerializer"/>.
		/// </remarks>
		protected virtual void Save(string alias, Configuration configuration)
		{
		}
	}
}
