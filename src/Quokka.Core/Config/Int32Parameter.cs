using System;

namespace Quokka.Config
{
	/// <summary>
	/// A Parameter with an integer value.
	/// </summary>
	public class Int32Parameter : ConfigParameter<int, Int32Parameter>
	{
		public Int32Parameter(string paramName)
			: base(paramName, ConfigParameterType.Int32)
		{
		}

		public override object ConvertFromString(string text)
		{
			return int.Parse(text);
		}

		public override string ConvertToString(object value)
		{
			return value.ToString();
		}

		public Int32Parameter WithValidRange(int lowerInclusive, int upperInclusive)
		{
			return WithValidation(n => n >= lowerInclusive && n <= upperInclusive
				                           ? null
				                           : string.Format("Value should be in the range {0} to {1} inclusive",
				                                           lowerInclusive,
				                                           upperInclusive));
		}
	}
}