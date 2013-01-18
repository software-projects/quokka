using System;

namespace Quokka.Config
{
	/// <summary>
	/// A Parameter with a URL as a value.
	/// </summary>
	public class UrlParameter : ConfigParameter<Uri, UrlParameter>
	{
		public UrlParameter(string paramName)
			: base(paramName, ConfigParameterType.Url)
		{
		}

		public bool TryParse(string s, out Uri result)
		{
			return Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out result);
		}

		public override object ConvertFromString(string text)
		{
			return new Uri(text);
		}

		public override string ConvertToString(object value)
		{
			return value.ToString();
		}
	}
}