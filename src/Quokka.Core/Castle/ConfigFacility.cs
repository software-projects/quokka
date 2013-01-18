using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Quokka.Config;
using Quokka.Config.Storage;

namespace Quokka.Castle
{
	/// <summary>
	/// The configuration facility provides integration between the configuration 
	/// classes provided in the Quokka.Config namespace and Castle Windsor.
	/// </summary>
	/// <remarks>
	/// <para>
	/// Add the <see cref="ConfigFacility"/> to your Castle Windsor container.
	/// </para>
	/// <para>
	/// Register a component that implements <see cref="IConfigStorage"/> that is capable of
	/// persisting configuration parameters.
	/// </para>
	/// <para>
	/// Define your configuration parameters as static, readonly fields. The configuration facility
	/// will find the parameters as necessary.
	/// </para>
	/// </remarks>
	public class ConfigFacility : AbstractFacility
	{
		private readonly object _lockObject = new object();
		private bool _foundStorage;
		private readonly HashSet<Assembly> _assemblies = new HashSet<Assembly>();
		private volatile Assembly _lastAssembly;

		protected override void Init()
		{
			Kernel.ComponentRegistered += CheckForConfigurationStorage;
			Kernel.ComponentRegistered += UpdateAssemblies;
			Kernel.Register(Component.For<IConfigRepo>().Instance(new Repo(this)));
		}

		private void CheckForConfigurationStorage(string key, IHandler handler)
		{
			foreach (var service in handler.ComponentModel.Services)
			{
				if (service.Is<IConfigStorage>())
				{
					lock (_lockObject)
					{
						if (!_foundStorage)
						{
							var storage = Kernel.Resolve<IConfigStorage>();
							var memoryStorage = storage as MemoryStorage;
							if (memoryStorage == null)
							{
								ConfigParameter.Storage = new MemoryStorage(storage);
							}
							else
							{
								ConfigParameter.Storage = storage;
							}

							// now that we are setup, don't need the event anymore
							Kernel.ComponentRegistered -= CheckForConfigurationStorage;
							_foundStorage = true;
						}
					}
				}
			}
		}

		private void UpdateAssemblies(string key, IHandler handler)
		{
			var assembly = handler.ComponentModel.Implementation.Assembly;

			// Optimization to reduce locking. Remember the last assembly added in a volatile
			// field, and only lock if not equal to the last assembly added. If there is a race
			// condition, then the worst that can happen is that the assembly will be added to the
			// set more often than is necessary.
			if (assembly != _lastAssembly)
			{
				_lastAssembly = assembly;
				lock (_lockObject)
				{
					_assemblies.Add(assembly);
				}
			}
		}

		private class Repo : IConfigRepo
		{
			private readonly ConfigFacility _facility;
			private List<ConfigParameter> _configParams;

			public Repo(ConfigFacility facility)
			{
				_facility = facility;
			}

			public IList<ConfigParameter> FindAll()
			{
				CreateListIfNecessary();
				return new ReadOnlyCollection<ConfigParameter>(_configParams);
			}

			public ConfigParameter FindByName(string name)
			{
				CreateListIfNecessary();
				return _configParams.Find(p => p.Name.EqualsText(name));
			}

			private void CreateListIfNecessary()
			{
				if (_configParams == null)
				{
					// By the time this method is called it would be highly unlikely that anything would be
					// still registering with the container, so just reuse the facility lock object because
					// we are accessing the facilities internals to build the list.
					lock (_facility._lockObject)
					{
						_configParams = new List<ConfigParameter>();
						foreach (var assembly in _facility._assemblies)
						{
							_configParams.AddRange(ConfigParameter.Find(assembly));
						}
						_configParams.Sort((p1, p2) => StringComparer.OrdinalIgnoreCase.Compare(p1.Name, p2.Name));
					}
				}
			}
		}
	}
}