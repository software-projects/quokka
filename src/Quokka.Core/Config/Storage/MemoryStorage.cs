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
		private readonly Dictionary<ConfigParameter, ConfigValue> _values 
			= new Dictionary<ConfigParameter, ConfigValue>();

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
						_values[parentValue.Parameter] = parentValue;
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
				_values[parameter] = new ConfigValue(parameter, value, true);
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
			ConfigValue value;
			if (_values.TryGetValue(parameter, out value))
			{
				return value;
			}

			if (Parent == null)
			{
				// No parent and no value
				return new ConfigValue(parameter, null, false);
			}

			value = Parent.GetValue(parameter);
			_values.Add(parameter, value);
			return value;
		}
	}
}