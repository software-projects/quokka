using System;
using Quantum.Constants;
using Quokka.Config.Internal;
using Quokka.Diagnostics;

namespace Quokka.Config
{
	/// <summary>
	/// Base class for a parameter specification.
	/// </summary>
	/// <remarks>
	/// A Parameter can be one of a number of different types: <see cref="StringParameter"/>, 
	/// <see cref="Int32Parameter"/>, <see cref="BooleanParameter"/>. This is the base class that
	/// has common properties for all of these different parameter types.
	/// </remarks>
	public abstract class Parameter
	{
		private readonly string _paramName;
		private readonly string _paramType;
		private readonly string _description;

		protected Parameter(string paramName, string paramType, string description)
		{
			_paramName = Verify.ArgumentNotNull(paramName, "paramName");
			_paramType = Verify.ArgumentNotNull(paramType, "paramType");
			_description = Verify.ArgumentNotNull(description, "description");
		}

		/// <summary>
		/// The name of the parameter.
		/// </summary>
		public string ParamName
		{
			get { return _paramName; }
		}

		/// <summary>
		/// String representation of the parameter type (eg "String", "Int32", "Boolean").
		/// </summary>
		public string ParamType
		{
			get { return _paramType; }
		}

		/// <summary>
		/// Brief description of the parameter.
		/// </summary>
		public string Description
		{
			get { return _description; }
		}

		public abstract string DefaultValueAsString { get; }
	}

	/// <summary>
	/// A parameter with a string value.
	/// </summary>
	public class StringParameter : Parameter
	{
		public StringParameter(string paramName, string description, string defaultValue = null)
			: this(paramName, ConfigParameterType.String, description, defaultValue)
		{
		}

		/// <summary>
		/// Constructor for derived types. These derived types add some semantic value to the string, but in the
		/// end they just return a string.
		/// </summary>
		protected StringParameter(string paramName, string paramType, string description, string defaultValue = null)
			: base(paramName, paramType, description)
		{
			DefaultValue = defaultValue;
		}

		public string DefaultValue { get; private set; }

		public override string DefaultValueAsString
		{
			get { return DefaultValue; }
		}
	}

	/// <summary>
	/// String parameter that contains the full path of a directory in the filesystem. Allows the value to be displayed differently
	/// in the maintenance UI, but it is really just a string parameter.
	/// </summary>
	public class DirectoryParameter : StringParameter
	{
		public DirectoryParameter(string paramName, string description, string defaultParameter = null)
			: base(paramName, ConfigParameterType.Directory, description, defaultParameter)
		{
		}
	}

	/// <summary>
	/// String parameter that contains a password. Essentially just a string parameter, but displayed slightly differently when listing
	/// config parameters. (Value is elided).
	/// </summary>
	public class PasswordParameter : StringParameter
	{
		public PasswordParameter(string paramName, string description)
			: base(paramName, ConfigParameterType.Password, description, null)
		{
		}
	}

	/// <summary>
	/// A Parameter with an integer value.
	/// </summary>
	public class Int32Parameter : Parameter
	{
		public Int32Parameter(string paramName, string description, int defaultValue = 0)
			: base(paramName, ConfigParameterType.Int32, description)
		{
			DefaultValue = defaultValue;
		}

		public int DefaultValue { get; private set; }

		public bool TryParse(string s, out int result)
		{
			return int.TryParse(s, out result);
		}

		public override string DefaultValueAsString
		{
			get { return DefaultValue.ToString(); }
		}
	}

	/// <summary>
	/// A Parameter with a boolean value.
	/// </summary>
	public class BooleanParameter : Parameter
	{
		public BooleanParameter(string paramName, string description, bool defaultValue = false)
			: base(paramName, ConfigParameterType.Boolean, description)
		{
			DefaultValue = defaultValue;
		}

		public bool DefaultValue { get; private set; }

		public bool TryParse(string s, out bool result)
		{
			if (bool.TryParse(s, out result))
			{
				return true;
			}

			if (string.IsNullOrWhiteSpace(s))
			{
				// Blank value is considered false
				result = false;
				return true;
			}

			// Could not parse, but the runtime library only checks for values "True" and "False".
			// Look for a few other non-ambiguous values.

			s = s.Trim().ToLowerInvariant();

			if (s == "y" || s == "yes" || s == "1" || s == "t")
			{
				result = true;
				return true;
			}

			if (s == "n" || s == "no" || s == "0" || s == "f")
			{
				result = false;
				return true;
			}

			return false;
		}

		public override string DefaultValueAsString
		{
			get { return DefaultValue.ToString(); }
		}
	}

	/// <summary>
	/// A Parameter with a date value (no time component -- only the date)
	/// </summary>
	public class DateParameter : Parameter
	{
		public DateParameter(string paramName, string description, DateTime defaultValue = default(DateTime))
			: base(paramName, ConfigParameterType.Date, description)
		{
			DefaultValue = defaultValue;
		}

		public DateTime DefaultValue { get; private set; }

		public override string DefaultValueAsString
		{
			get { return DefaultValue.ToString("yyyy-MM-dd"); }
		}

		public bool TryParse(string s, out DateTime result)
		{
			return DateTime.TryParse(s, out result);
		}
	}

	/// <summary>
	/// A Parameter with a URL as a value.
	/// </summary>
	public class UrlParameter : Parameter
	{
		public UrlParameter(string paramName, string description, Uri defaultValue = default(Uri))
			: base(paramName, ConfigParameterType.Url, description)
		{
			DefaultValue = defaultValue;
		}

		public Uri DefaultValue { get; private set; }

		public override string DefaultValueAsString
		{
			get { return DefaultValue == null ? null : DefaultValue.ToString(); }
		}

		public bool TryParse(string s, out Uri result)
		{
			return Uri.TryCreate(s, UriKind.RelativeOrAbsolute, out result);
		}
	}

	/// <summary>
	/// A Parameter with a time span value
	/// </summary>
	public class TimeSpanParameter : Parameter
	{
		public TimeSpanParameter(string paramName, string description, TimeSpan defaultValue = default(TimeSpan))
			: base(paramName, ConfigParameterType.TimeSpan, description)
		{
			DefaultValue = defaultValue;
		}

		public TimeSpan DefaultValue { get; private set; }

		public bool TryParse(string s, out TimeSpan result)
		{
			return TimeSpanExtensions.TryParse(s, out result);
		}

		public override string DefaultValueAsString
		{
			get { return DefaultValue.ToReadableString(); }
		}
	}
}