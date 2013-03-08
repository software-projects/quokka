using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Core.Internal;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
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

		protected override void Init()
		{
			Kernel.ComponentRegistered += CheckForConfigurationStorage;
			Kernel.ComponentRegistered += UpdateAssemblies;
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
			if (_assemblies.Add(assembly))
			{
				ConfigParameter.All.AddFromAssembly(assembly);
			}
		}
	}
}