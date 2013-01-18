using System;
using System.Collections.Generic;

namespace Quokka.Config.Storage
{
	/// <summary>
	/// Configuration storage where all configuration is stored in memory.
	/// Can have an optional parent storage, in which case this acts as a 
	/// caching storage.
	/// </summary>
	public class MemoryStorage : IConfigStorage
	{
		private readonly Dictionary<string, string> _values = new Dictionary<string, string>();

		// TODO: make for better concurrency with read-write lock
		private readonly object _lockObject = new object();

		public IConfigStorage Parent { get; private set; }

		public ConfigValue GetValue(ConfigParameter parameter)
		{
			lock (_lockObject)
			{
				return GetValueWithoutLock(parameter);
			}
		}

		public ConfigValue[] GetValues(params ConfigParameter[] parameters)
		{
			if (Parent != null)
			{
				// delegate entirely to the parent
				var parentValues = Parent.GetValues(parameters);

				// update the dictionary with values from the parent
				lock (_lockObject)
				{
					foreach (var parentValue in parentValues)
					{
						if (parentValue.HasValue)
						{
							_values[parentValue.Parameter.Name] = parentValue.Value;
						}
						else
						{
							_values.Remove(parentValue.Parameter.Name);
						}
					}
				}

				return parentValues;
			}

			// No parent, so our values are the single point of truth.
			var values = new ConfigValue[parameters.Length];
			lock (_lockObject)
			{
				for (int index = 0; index < values.Length; ++index)
				{
					values[index] = GetValueWithoutLock(parameters[index]);
				}
			}
			return values;
		}

		public bool IsReadOnly
		{
			get
			{
				if (Parent != null)
				{
					return Parent.IsReadOnly;
				}
				return false;
			}
		}

		public void SetValue(ConfigParameter parameter, string value)
		{
			lock (_lockObject)
			{
				if (Parent != null)
				{
					Parent.SetValue(parameter, value);
				}
				_values[parameter.Name] = value;
			}
		}

		public void Refresh()
		{
			if (Parent != null)
			{
				// Only clear if the parent exists, because if there is no parent
				// we are the only source of configuration information (which should
				// only happen during unit testing).
				lock (_lockObject)
				{
					_values.Clear();
				}
			}
		}

		public MemoryStorage(IConfigStorage parent = null)
		{
			Parent = parent;
		}

		private ConfigValue GetValueWithoutLock(ConfigParameter parameter)
		{
			string value;
			bool hasValue = _values.TryGetValue(parameter.Name, out value);
			if (!hasValue && Parent != null)
			{
				var configValue = Parent.GetValue(parameter);
				if (configValue.HasValue)
				{
					_values.Add(parameter.Name, configValue.Value);
					return configValue;
				}
			}

			return new ConfigValue(parameter, value, hasValue);
		}
	}
}