using System;
using System.Collections.Generic;
using System.Data;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// Provides the specification for building a type derived from <see cref="DataRecordConverter"/>. Any two
	/// <see cref="SqlQuery"/> classes that generate equivalent <see cref="DataRecordConverterSpec"/> objects
	/// can use the same <see cref="DataRecordConverter"/> derived type.
	/// </summary>
	/// <remarks>
	/// This class is designed to act as a type because it is immutable, and two instances with identical 
	/// values are considered equal. It is used as the key for dictionary collections.
	/// </remarks>
	internal class DataRecordConverterSpec
	{
		private readonly IList<DataRecordFieldInfo> _fields;
		private readonly Type _recordType;

		public DataRecordConverterSpec(IDataRecord dataRecord, Type recordType)
		{
			var fields = new List<DataRecordFieldInfo>(dataRecord.FieldCount);
			for (int index = 0; index < dataRecord.FieldCount; ++index)
			{
				fields.Add(new DataRecordFieldInfo(index, dataRecord.GetName(index), dataRecord.GetFieldType(index)));
			}

			_recordType = recordType;
			_fields = fields.AsReadOnly();
		}

		public Type RecordType
		{
			get { return _recordType; }
		}

		public IList<DataRecordFieldInfo> Fields
		{
			get { return _fields; }
		}

		public override bool Equals(object obj)
		{
			var other = obj as DataRecordConverterSpec;
			if (other == null)
			{
				return false;
			}

			if (other.RecordType != RecordType)
			{
				return false;
			}

			if (other.Fields.Count != Fields.Count)
			{
				return false;
			}

			for (int index = Fields.Count - 1; index >= 0; --index)
			{
				if (other.Fields[index] != Fields[index])
				{
					return false;
				}
			}

			return true;
		}

		public override int GetHashCode()
		{
			return RecordType.GetHashCode();
		}
	}
}