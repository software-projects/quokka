using System;
using System.Transactions;
using Quokka.Diagnostics;

namespace Quokka.Config.Implementation
{
	/// <summary>
	/// Provides a base class for implementing <see cref="IConfig"/>
	/// </summary>
	public abstract class ConfigBase : IConfig
	{
		public string GetValue(StringParameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			string stringValue;

			if (TryGetStringValue(parameter, out stringValue))
			{
				return stringValue;
			}

			return parameter.DefaultValue;
		}

		public int GetValue(Int32Parameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			string stringValue;

			if (TryGetStringValue(parameter, out stringValue))
			{
				int result;
				if (parameter.TryParse(stringValue, out result))
				{
					return result;
				}
			}

			return parameter.DefaultValue;
		}

		public bool GetValue(BooleanParameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			string stringValue;

			if (TryGetStringValue(parameter, out stringValue))
			{
				bool result;
				if (parameter.TryParse(stringValue, out result))
				{
					return result;
				}
			}

			return parameter.DefaultValue;
		}

		public Uri GetValue(UrlParameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			string stringValue;

			if (TryGetStringValue(parameter, out stringValue))
			{
				Uri result;
				if (parameter.TryParse(stringValue, out result))
				{
					return result;
				}
			}

			return parameter.DefaultValue;
		}

		public DateTime GetValue(DateParameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			string stringValue;

			if (TryGetStringValue(parameter, out stringValue))
			{
				DateTime result;
				if (parameter.TryParse(stringValue, out result))
				{
					return result;
				}
			}

			return parameter.DefaultValue;
		}

		public TimeSpan GetValue(TimeSpanParameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			string stringValue;

			if (TryGetStringValue(parameter, out stringValue))
			{
				TimeSpan result;
				if (parameter.TryParse(stringValue, out result))
				{
					return result;
				}
			}

			return parameter.DefaultValue;
		}

		/// <summary>
		/// Attempt to get value for the configuration parameter as a string value.
		/// </summary>
		/// <param name="parameter">The parameter</param>
		/// <param name="result">Returns the parameter value as a string</param>
		/// <returns>
		/// Returns <c>true</c> if found, <c>false</c> otherwise. If not found, the default
		/// value will be used.
		/// </returns>
		protected abstract bool TryGetStringValue(Parameter parameter, out string result);

		protected virtual bool SetStringValue(Parameter parameter, string stringValue, bool overwriteIfExists)
		{
			throw new NotSupportedException("Writing configuration parameter values is not supported");
		}

		protected virtual void Publish(ConfigChangedMessage message)
		{
			throw new NotSupportedException("Publishing ConfigChangedMessage messages is not supported");
		}

		/// <summary>
		/// Indicates whether write operations are supported by this implementation.
		/// </summary>
		/// <remarks>
		/// Subclasses must set this property to <c>true</c> if they support write operations.
		/// </remarks>
		public bool CanWrite { get; set; }

		private void Publish(Parameter parameter, string stringValue)
		{
			var message = new ConfigChangedMessage
			              	{
			              		ParamName = parameter.ParamName,
			              		ParamType = parameter.ParamType,
			              		ParamValue = stringValue,
			              	};

			// Publish to subscribers -- if a transaction is current, the message will
			// be sent when the transaction is committed.
			Publish(message);
		}

		private void VerifyWriteSupported()
		{
			if (!CanWrite)
			{
				throw new NotSupportedException("Write operations not supported");
			}
		}

		public void Register(Parameter parameter)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			VerifyWriteSupported();
			var stringValue = parameter.DefaultValueAsString;
			if (SetStringValue(parameter, stringValue, false))
			{
				Publish(parameter, stringValue);
			}
		}

		public void SetValue(StringParameter parameter, string value)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			VerifyWriteSupported();
			if (SetStringValue(parameter, value, true))
			{
				Publish(parameter, value);
			}
		}

		public void SetValue(Int32Parameter parameter, int value)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			VerifyWriteSupported();
			var stringValue = value.ToString();
			if (SetStringValue(parameter, stringValue, true))
			{
				Publish(parameter, stringValue);
			}
		}

		public void SetValue(BooleanParameter parameter, bool value)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			VerifyWriteSupported();
			var stringValue = value.ToString();
			if (SetStringValue(parameter, stringValue, true))
			{
				Publish(parameter, stringValue);
			}
		}

		public void SetValue(UrlParameter parameter, Uri value)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			VerifyWriteSupported();
			var stringValue = value == null ? null : value.ToString();
			if (SetStringValue(parameter, stringValue, true))
			{
				Publish(parameter, stringValue);
			}
		}

		public void SetValue(DateParameter parameter, DateTime value)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			VerifyWriteSupported();
			var stringValue = parameter.ConvertToString(value);
			if (SetStringValue(parameter, stringValue, true))
			{
				Publish(parameter, stringValue);
			}
		}

		public void SetValue(TimeSpanParameter parameter, TimeSpan value)
		{
			Verify.ArgumentNotNull(parameter, "parameter");
			VerifyWriteSupported();
			var stringValue = parameter.ConvertToString(value);
			if (SetStringValue(parameter, stringValue, true))
			{
				Publish(parameter, stringValue);
			}
		}
	}
}