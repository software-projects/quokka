using System;

namespace Quokka.Config.Implementation
{
	/// <summary>
	/// Provides a base class for implementing <see cref="IConfig"/>
	/// </summary>
	public abstract class ConfigBase : IConfig
	{
		public string GetValue(StringParameter parameter)
		{
			string stringValue;

			if (TryGetStringValue(parameter, out stringValue))
			{
				return stringValue;
			}

			return parameter.DefaultValue;
		}

		public int GetValue(Int32Parameter parameter)
		{
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
	}
}
