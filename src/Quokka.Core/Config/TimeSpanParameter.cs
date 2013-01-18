﻿using System;

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

		public string ConvertToString(TimeSpan timeSpan)
		{
			return timeSpan.ToReadableString();
		}

		public override object ConvertFromString(string text)
		{
			TimeSpan value;
			if (!TimeSpanExtensions.TryParse(text, out value))
			{
				throw new FormatException("Invalid timespan value");
			}
			return value;
		}

		public override string ConvertToString(object value)
		{
			return ((TimeSpan) value).ToReadableString();
		}
	}
}