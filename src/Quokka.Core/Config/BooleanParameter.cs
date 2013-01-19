using System;
using Quokka.Util;

namespace Quokka.Config
{
	/// <summary>
	/// A Parameter with a boolean value.
	/// </summary>
	public class BooleanParameter : ConfigParameter<bool, BooleanParameter>
	{
		public BooleanParameter(string paramName)
			: base(paramName, ConfigParameterType.Boolean)
		{
		}

		protected override object ConvertFromString(string s)
		{
			// check expected values first
			if (s == "1")
			{
				return true;
			}
			if (s == "0")
			{
				return false;
			}

			// attempt to parse using runtime library
			{
				bool result;
				if (bool.TryParse(s, out result))
				{
					return result;
				}
			}

			if (StringUtils.IsNullOrWhiteSpace(s))
			{
				// Blank value is considered false
				return false;
			}

			// Could not parse, but the runtime library only checks for values "True" and "False".
			// Look for a few other non-ambiguous values.

			s = s.Trim().ToLowerInvariant();

			if (s == "y" || s == "yes" || s == "1" || s == "t")
			{
				return true;
			}

			if (s == "n" || s == "no" || s == "0" || s == "f")
			{
				return false;
			}

			// don't know what it is, return false
			return false;
		}


		protected override string ConvertToString(object value)
		{
			return ((bool)value) ? "1" : "0";
		}
	}
}
