using System;

namespace Quokka.Config
{
	/// <summary>
	/// A Parameter with a time span value
	/// </summary>
	public class TimeSpanParameter : ConfigParameter<TimeSpan, TimeSpanParameter>
	{
		public TimeSpanParameter(string paramName) : base(paramName, ConfigParameterType.TimeSpan)
		{
		}

		protected override object ConvertFromString(string text)
		{
			TimeSpan value;
			if (!TimeSpanExtensions.TryParse(text, out value))
			{
				throw new FormatException("Invalid timespan value");
			}
			return value;
		}

		protected override string ConvertToString(object value)
		{
			return ((TimeSpan) value).ToReadableString();
		}
	}
}