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
	}
}