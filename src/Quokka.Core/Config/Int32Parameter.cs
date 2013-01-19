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

		protected override object ConvertFromString(string text)
		{
			return int.Parse(text);
		}

		protected override string ConvertToString(object value)
		{
			return value.ToString();
		}
	}

	public static class Int32ParameterExtensions
	{
		public static IConfigParameterBuilder<int> ValidRange(this IConfigParameterBuilder<int> @this, int lowerInclusive, int upperInclusive)
		{
			return @this.Validation(n => n >= lowerInclusive && n <= upperInclusive
										   ? null
										   : string.Format("Value should be in the range {0} to {1} inclusive",
														   lowerInclusive,
														   upperInclusive));
		}
	}
}