using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Quokka.Attributes;
using Quokka.Diagnostics;

namespace Quokka.EnumTypes
{
	public static class EnumStringConverter<T>
	{
		private static readonly ILogger log = NullLogger.Instance; // TODO: hook this up somehow?
		private static readonly int _maxLength;
		private static readonly IDictionary<T, string> _enumToString;
		private static readonly IDictionary<string, T> _stringToEnum;
		private static readonly T _nullValue;
		private static readonly bool _hasNullValue;
		private static readonly Type _enumType;

		public static int MaxLength
		{
			get { return _maxLength; }
		}

		public static string ConvertToString(T enumValue)
		{
			if (_hasNullValue && _nullValue.Equals(enumValue))
				return null;
			try
			{
				return _enumToString[enumValue];
			}
			catch (KeyNotFoundException)
			{
				string message = String.Format("Unknown enum value for type {0}: {1}", _enumType, enumValue);
				log.Error(message);
				throw new ApplicationException(message);
			}
		}

		public static T ConvertToEnum(string stringValue)
		{
			if (stringValue == null)
			{
				if (_hasNullValue)
				{
					return _nullValue;
				}
				return default(T);
			}

			try
			{
				return _stringToEnum[stringValue];
			}
			catch (KeyNotFoundException)
			{
				string message = String.Format("Unknown string value for type {0}: {1}", _enumType, stringValue);
				log.Error(message);
				// TODO: define a suitable exception
				throw new ApplicationException(message);
			}
		}

		private class EqualityComparer : IEqualityComparer<string>
		{
			public bool Equals(string x, string y)
			{
				return CaseInsensitiveComparer.DefaultInvariant.Compare(x, y) == 0;
			}

			public int GetHashCode(string s)
			{
				return s.ToUpperInvariant().GetHashCode();
			}
		}

		static EnumStringConverter()
		{
			_enumToString = new Dictionary<T, string>();
			_stringToEnum = new Dictionary<string, T>(new EqualityComparer());

			_enumType = typeof(T);
			if (!_enumType.IsEnum)
			{
				string message = "Type is not an enum: " + _enumType;
				log.Error(message);
				throw new ApplicationException(message);
			}

			foreach (T enumValue in Enum.GetValues(_enumType))
			{
				string stringValue = FindStringValueForEnum(enumValue);

				if (stringValue == null)
				{
					if (_hasNullValue)
					{
						string message =
							String.Format("Two {0} fields have null StringValue: {1}, {2}", _enumType, _nullValue,
										  enumValue);
						log.Error(message);
						throw new ApplicationException(message);
					}
					_hasNullValue = true;
					_nullValue = enumValue;
				}
				else
				{
					if (stringValue.Length > _maxLength)
					{
						_maxLength = stringValue.Length;
					}
					try
					{
						_stringToEnum.Add(stringValue, enumValue);
					}
					catch (ArgumentException)
					{
						string message =
							String.Format("Two {0} fields have the same StringValue: {1}, {2}", _enumType,
										  _stringToEnum[stringValue], enumValue);
						log.Error(message);
						throw new ApplicationException(message);
					}

					_enumToString.Add(enumValue, stringValue);
				}
			}
		}

		private static string FindStringValueForEnum(T enumValue)
		{
			string fieldName = enumValue.ToString();
			FieldInfo fieldInfo = _enumType.GetField(fieldName);
			StringValueAttribute[] stringValueAttributes =
				(StringValueAttribute[])fieldInfo.GetCustomAttributes(typeof(StringValueAttribute), false);
			if (stringValueAttributes.Length == 0)
			{
				// When there is no StringValue attribute, just return the string representation
				return fieldName;
			}
			return stringValueAttributes[0].StringValue;
		}
	}
}
