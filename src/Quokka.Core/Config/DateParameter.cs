using System;

namespace Quokka.Config
{
	/// <summary>
	/// A Parameter with a date value (no time component -- only the date)
	/// </summary>
	public class DateParameter : ConfigParameter<DateTime, DateParameter>
	{
		public DateParameter(string paramName)
			: base(paramName, ConfigParameterType.Date)
		{
		}

		public override object ConvertFromString(string text)
		{
			return DateTime.Parse(text);
		}

		public override string ConvertToString(object value)
		{
			return ((DateTime) value).ToString("yyyy-MM-dd");
		}
	}
}