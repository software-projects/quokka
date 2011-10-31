#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion

using System;
using System.Collections.Generic;
using System.Data;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// 	Base class for types that convert data from an <see cref = "IDataRecord" /> instance into
	/// 	a record instance whose class type is defined as the generic parameter T to the <see cref = "SqlQuery{T}" /> class.
	/// </summary>
	/// <remarks>
	/// 	The <see cref = "DataRecordConverterTypeBuilder" /> creates dynamic types that derive from this class.
	/// </remarks>
	public abstract class DataRecordConverter
	{
		private static readonly Dictionary<DataRecordConverterSpec, Type> ConverterTypes =
			new Dictionary<DataRecordConverterSpec, Type>();

		private readonly IDataReader _dataReader;

		protected DataRecordConverter(IDataReader dataReader)
		{
			if (dataReader == null)
			{
				throw new ArgumentNullException("dataReader");
			}
			_dataReader = dataReader;
		}

		public static DataRecordConverter CreateConverterFor(Type recordType, IDataRecord dataRecord)
		{
			Type type;

			var key = new DataRecordConverterSpec(dataRecord, recordType);
			if (!ConverterTypes.TryGetValue(key, out type))
			{
				// TODO: make this thread-safe
				var builder = new DataRecordConverterTypeBuilder(key);
				type = builder.BuildConverter();
				ConverterTypes.Add(key, type);
			}

			object obj = Activator.CreateInstance(type, dataRecord);
			return (DataRecordConverter) obj;
		}

		protected abstract void DoCopy(object record);

		public void CopyTo(object record)
		{
			if (record == null)
			{
				throw new ArgumentNullException("record");
			}

			DoCopy(record);
		}

		#region Protected get methods

		protected bool GetBoolean(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(bool);
			}
			return _dataReader.GetBoolean(index);
		}

		protected bool? GetNullableBoolean(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetBoolean(index);
		}

		protected byte GetByte(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(byte);
			}
			return _dataReader.GetByte(index);
		}

		protected byte? GetNullableByte(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetByte(index);
		}

		protected char GetChar(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(char);
			}
			return _dataReader.GetChar(index);
		}

		protected char? GetNullableChar(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetChar(index);
		}

		protected DateTime GetDateTime(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(DateTime);
			}
			return _dataReader.GetDateTime(index);
		}

		protected DateTime? GetNullableDateTime(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetDateTime(index);
		}

		protected decimal GetDecimal(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(decimal);
			}
			return _dataReader.GetDecimal(index);
		}

		protected decimal? GetNullableDecimal(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetDecimal(index);
		}

		protected double GetDouble(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(double);
			}
			return _dataReader.GetDouble(index);
		}

		protected double? GetNullableDouble(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetDouble(index);
		}

		protected float GetFloat(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(float);
			}
			return _dataReader.GetFloat(index);
		}

		protected float? GetNullableFloat(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetFloat(index);
		}

		protected Guid GetGuid(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(Guid);
			}
			return _dataReader.GetGuid(index);
		}

		protected Guid? GetNullableGuid(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetGuid(index);
		}

		protected short GetInt16(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(short);
			}
			return _dataReader.GetInt16(index);
		}

		protected short? GetNullableInt16(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetInt16(index);
		}

		protected int GetInt32(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(int);
			}
			return _dataReader.GetInt32(index);
		}

		protected int GetInt32FromNumeric(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(int);
			}
			return Convert.ToInt32(_dataReader.GetValue(index));
		}

		protected int? GetNullableInt32FromNumeric(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return Convert.ToInt32(_dataReader.GetValue(index));
		}

		protected int? GetNullableInt32(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetInt32(index);
		}

		protected long GetInt64(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(long);
			}
			return _dataReader.GetInt32(index);
		}

		protected long? GetNullableInt64(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetInt64(index);
		}

		protected string GetString(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return _dataReader.GetString(index);
		}

		protected byte[] GetBytes(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}
			return (byte[]) _dataReader.GetValue(index);
		}

		protected T GetEnumFromString<T>(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(T);
			}

			string text = (_dataReader.GetString(index) ?? string.Empty).Trim();
			if (String.IsNullOrEmpty(text))
			{
				return default(T);
			}

			// TODO: could use an enum/string mapper class to convert based on [Description] attribute.
			return (T) Enum.Parse(typeof (T), text);
		}

		protected T? GetNullableEnumFromString<T>(int index) where T : struct
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}

			string text = (_dataReader.GetString(index) ?? string.Empty).Trim();
			if (String.IsNullOrEmpty(text))
			{
				return null;
			}

			// TODO: could use an enum/string mapper class to convert based on [Description] attribute.
			return (T) Enum.Parse(typeof (T), text);
		}

		protected T GetEnumFromInt32<T>(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(T);
			}

			int intValue = _dataReader.GetInt32(index);
			return (T) Enum.ToObject(typeof (T), intValue);
		}

		protected T? GetNullableEnumFromInt32<T>(int index) where T : struct
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}

			int intValue = _dataReader.GetInt32(index);
			return (T) Enum.ToObject(typeof (T), intValue);
		}

		protected T GetEnumFromNumber<T>(int index)
		{
			if (_dataReader.IsDBNull(index))
			{
				return default(T);
			}

			object numValue = _dataReader.GetValue(index);
			return (T) Enum.ToObject(typeof (T), numValue);
		}

		protected T? GetNullableEnumFromNumber<T>(int index) where T : struct
		{
			if (_dataReader.IsDBNull(index))
			{
				return null;
			}

			object numValue = _dataReader.GetValue(index);
			return (T) Enum.ToObject(typeof (T), numValue);
		}

		#endregion
	}
}