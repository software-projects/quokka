using System;
using System.Collections.Generic;
using Quokka.Diagnostics;
using Quokka.Sprocket;

namespace Quokka.Config.Implementation
{
	/// <summary>
	/// Provides a base class for implementing <see cref="IConfig"/>, where config values are cached locally.
	/// </summary>
	/// <remarks>
	/// Subscribes to <see cref="ConfigChangedMessage"/>, and clears the appropriate values
	/// from the cache when the message is published.
	/// </remarks>
	public abstract class CachingConfigBase : ConfigBase, IDisposable
	{
		private readonly Dictionary<string, string> _cachedValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
		private readonly object _lockObject = new object();
		private IDisposable _subscription;
		private readonly ISprocket _sprocket;

		protected CachingConfigBase(ISprocket sprocket)
		{
			// Do not subscribe to the message straight away, because
			_sprocket = Verify.ArgumentNotNull(sprocket, "sprocket");

			if (_sprocket.Connected)
			{
				Subscribe();
			}
			else
			{
				// If we are not connected yet, there is the possibility that the sprocket connection
				// has not been opened yet. (Flaw in the current implementation that you cannot create
				// a subscription before opening the client).
				_sprocket.ConnectedChanged += SprocketConnectedChanged;
			}
		}

		private void Subscribe()
		{
			_subscription = _sprocket.CreateSubscriber<ConfigChangedMessage>().WithAction(ConfigValueChangedHandler);
		}

		private void SprocketConnectedChanged(object sender, EventArgs e)
		{
			lock (_lockObject)
			{
				if (_subscription == null)
				{
					Subscribe();
					_sprocket.ConnectedChanged -= SprocketConnectedChanged;
				}
			}
		}

		public void Dispose()
		{
			_subscription.Dispose();
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

		public void ConfigValueChangedHandler(ConfigChangedMessage message)
		{
			lock (_lockObject)
			{
				if (message == null || string.IsNullOrWhiteSpace(message.ParamName))
				{
					_cachedValues.Clear();
				}
				else
				{
					_cachedValues.Remove(message.ParamName);
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

			var found = TryGetStringValueAfterCacheMiss(parameter, out result);
			if (found)
			{
				lock (_lockObject)
				{
					_cachedValues[parameter.ParamName] = result;
				}
			}

			return found;
		}

		protected override void Publish(ConfigChangedMessage message)
		{
			_sprocket.Publish(message);
		}

		protected abstract bool TryGetStringValueAfterCacheMiss(Parameter parameter, out string result);
	}
}
