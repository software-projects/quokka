using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Castle.Core;
using Castle.MicroKernel;
using Castle.MicroKernel.Facilities;
using Castle.MicroKernel.Registration;
using Quokka.Config;
using Quokka.Diagnostics;
using ILogger = Castle.Core.Logging.ILogger;
using NullLogger = Castle.Core.Logging.NullLogger;
using Parameter = Quokka.Config.Parameter;

namespace Quokka.Server.Internal
{
	public class ConfigParamFacility : AbstractFacility
	{
		private readonly ConfigAssemblyCollection _assemblies = new ConfigAssemblyCollection();

		protected override void Init()
		{
			Kernel.Register(Component.For<IStartable>().ImplementedBy<ConfigParamRegistration>().LifeStyle.Singleton);
			Kernel.Register(Component.For<ConfigAssemblyCollection>().Instance(_assemblies));
			Kernel.ComponentRegistered += KernelComponentRegistered;
		}

		private void KernelComponentRegistered(string key, IHandler handler)
		{
			var componentModel = handler.ComponentModel;
			var service = handler.Service;

			var assembly = componentModel.Implementation.Assembly;
			_assemblies.AddAssembly(assembly);
		}
	}

	internal class ConfigAssemblyCollection
	{
		private readonly object _lockObject = new object();
		private readonly Queue<Assembly> _queue = new Queue<Assembly>();
		private readonly HashSet<Assembly> _set = new HashSet<Assembly>();

		public event EventHandler AssemblyAdded;

		public ConfigAssemblyCollection()
		{
			// Add subscription to event so we don't have to check for zero
			AssemblyAdded += delegate { };
		}

		public Assembly GetNextAssembly()
		{
			lock (_lockObject)
			{
				if (_queue.Count == 0)
				{
					return null;
				}
				return _queue.Dequeue();
			}
		}

		public void AddAssembly(Assembly assembly)
		{
			lock (_lockObject)
			{
				if (_set.Contains(assembly))
				{
					// Already added so no need to add again
					return;
				}

				_set.Add(assembly);
				_queue.Enqueue(assembly);
			}

			ThreadPool.QueueUserWorkItem(delegate { AssemblyAdded(this, EventArgs.Empty); });
		}
	}

	internal class ConfigParamRegistration : IStartable
	{
		private readonly IConfig _config;
		private readonly ConfigAssemblyCollection _assemblies;
		private readonly HashSet<Assembly> _processedAssemblies = new HashSet<Assembly>();

		public ILogger Logger { get; set;}

		public ConfigParamRegistration(IConfig config, ConfigAssemblyCollection assemblies)
		{
			Logger = NullLogger.Instance;
			_config = Verify.ArgumentNotNull(config, "config");
			_assemblies = Verify.ArgumentNotNull(assemblies, "assemblies");

			_assemblies.AssemblyAdded += delegate { ProcessAssemblies(); };
		}

		public void Start()
		{

			ProcessAssemblies();
		}

		public void Stop()
		{
		}

		private void ProcessAssemblies()
		{
			for (;;)
			{
				var assembly = _assemblies.GetNextAssembly();
				if (assembly == null || _processedAssemblies.Contains(assembly))
				{
					break;
				}

				ProcessAssembly(assembly);
				_processedAssemblies.Add(assembly);
			}
		}

		private readonly string[] _reservedNames = new[]
		                                           	{
														"mscorlib",
		                                           		"System",
		                                           		"Castle.",
		                                           		"NHibernate",
		                                           		"Quartz",
		                                           		"log4net",
		                                           		"Microsoft.",
		                                           		"Quokka",
														"Common.",
														"Iesi",
														"Nlog",
		                                           	};

		private bool IsReservedName(string name)
		{
			return _reservedNames.Any(reservedName => name.StartsWith(reservedName, StringComparison.OrdinalIgnoreCase));
		}

		private void ProcessAssembly(Assembly assembly)
		{
			if (!_config.CanWrite)
			{
				// No point if the config does not support write
				return;
			}

			// Eliminate known assemblies that we do not want to process
			var name = assembly.GetName().Name;
			if (IsReservedName(name))
			{
				return;
			}

			Logger.Debug("Processing config parameters in " + assembly);

			var fields = from t in assembly.GetTypes()
						 from f in t.GetFields()
						 where f.IsStatic
						 where typeof(Parameter).IsAssignableFrom(f.FieldType)
						 select f;

			foreach (var field in fields)
			{
				var parameter = field.GetValue(null) as Parameter;
				if (parameter == null)
				{
					Logger.WarnFormat("Ignoring parameter defined in {0}.{1} because its value is null", field.DeclaringType.FullName,
					                  field.Name);
					continue;
				}

				if (!field.IsInitOnly)
				{
					Logger.WarnFormat("Ignoring parameter {0} defined in {1}.{2} because it is not defined as readonly",
					                  parameter.ParamName,
					                  field.DeclaringType.FullName,
					                  field.Name);
					continue;
				}

				_config.Register(parameter);
			}

			// now that the assembly has loaded all types, any referenced assembly should be loaded
			foreach (var assemblyName in assembly.GetReferencedAssemblies())
			{
				if (!IsReservedName(assemblyName.Name))
				{
					var referencedAssembly = Assembly.Load(assemblyName);
					_assemblies.AddAssembly(referencedAssembly);
				}
			}
		}
	}
}
