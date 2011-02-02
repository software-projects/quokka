using System;
using System.Collections.Generic;
using System.Data;

namespace Quokka.Data.DataReader
{
	/// <summary>
	/// 	This class can be used to 'decorate' an <see cref = "IDataReader" /> so
	/// 	that all string fields have their trailing spaces trimmed.
	/// </summary>
	public class StringTrimDataReader : DataReaderDecorator
	{
		private readonly HashSet<int> _stringColumns = new HashSet<int>();
		private readonly HashSet<int> _dateTimeColumns = new HashSet<int>();

		public StringTrimDataReader(IDataReader inner)
			: base(inner)
		{
			int count = Inner.FieldCount;
			for (int index = 0; index < count; index++)
			{
				Type t = Inner.GetFieldType(index);
				if (t == typeof (string))
				{
					_stringColumns.Add(index);
				}
				else if (t == typeof (DateTime))
				{
					_dateTimeColumns.Add(index);
				}
			}
		}

		public override object GetValue(int i)
		{
			return SanitizeValue(base.GetValue(i), i);
		}

		public override int GetValues(object[] values)
		{
			int result = Inner.GetValues(values);
			for (int index = 0; index < result; ++index)
			{
				values[index] = SanitizeValue(values[index], index);
			}

			return result;
		}

		public override string GetString(int i)
		{
			return SanitizeString(Inner.GetString(i));
		}

		public override DateTime GetDateTime(int i)
		{
			return SanitizeDateTime(base.GetDateTime(i));
		}

		private object SanitizeValue(object value, int i)
		{
			if (_stringColumns.Contains(i))
			{
				return SanitizeString((string) value);
			}
			if (_dateTimeColumns.Contains(i))
			{
				return SanitizeDateTime((DateTime) value);
			}
			return value;
		}

		private static string SanitizeString(string s)
		{
			if (s == null)
			{
				return null;
			}
			return s.TrimEnd();
		}

		private static readonly DateTime NullDate = new DateTime(1899, 12, 30);

		private static DateTime SanitizeDateTime(DateTime dt)
		{
			if (dt.Year < 1900)
			{
				return NullDate;
			}
			return dt;
		}
	}
}