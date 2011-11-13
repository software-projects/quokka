using System;
using System.Collections.Generic;
using Quokka.Diagnostics;
using Quokka.Events;

namespace Quokka.Config.Implementation
{
	/// <summary>
	/// Provides a base class for implementing <see cref="IConfig"/>, where config values are cached
	/// locally.
	/// </summary>
	/// <remarks>
	/// Subscribes to the <see cref="ConfigValueChangedEvent"/>, and clears the appropriate values
	/// from the cache when the event is published.
	/// </remarks>
	public abstract class CachingConfigBase : ConfigBase, IDisposable
	{
		private readonly Dictionary<string, string> _cachedValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private readonly object _lockObject = new object();
		private readonly IEventSubscription _eventSubscription;

		protected CachingConfigBase(IEventBroker eventBroker)
		{
			Verify.ArgumentNotNull(eventBroker, "eventBroker");
			_eventSubscription = eventBroker.GetEvent<ConfigValueChangedEvent>()
				.Subscribe(ClearCache);
		}

		public void Dispose()
		{
			_eventSubscription.Dispose();
		}

		public void ClearCache()
		{
			lock(_lockObject)
			{
				_cachedValues.Clear();
			}
		}

		public void ClearCache(Parameter parameter)
		{
			lock(_lockObject)
			{
				if (parameter == null)
				{
					_cachedValues.Clear();
				}
				else
				{
					_cachedValues.Remove(parameter.ParamName);
				}
			}
		}

		public void ClearCache(string paramName)
		{
			lock (_lockObject)
			{
				if (paramName == null)
				{
					_cachedValues.Clear();
				}
				else
				{
					_cachedValues.Remove(paramName);
				}
			}
		}

		protected override bool TryGetStringValue(Parameter parameter, out string result)
		{
			Verify.ArgumentNotNull(parameter, "parameter");

			lock (_lockObject)
			{
				if (_cachedValues.TryGetValue(parameter.ParamName, out result))
				{
					return true;
				}
			}

			var found = TryGetStringValueAfterCacheMiss(parameter.ParamName, out result);
			if (found)
			{
				lock (_lockObject)
				{
					_cachedValues[parameter.ParamName] = result;
				}
			}

			return found;
		}


		protected abstract bool TryGetStringValueAfterCacheMiss(string paramName, out string result);
	}
}
