using System;
using System.Data;
using System.Text;
using Quokka.Diagnostics;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// Contains information about a field in an <see cref="IDataRecord"/>. 
	/// </summary>
	/// <remarks>
	/// This type can be used as a value type in the sense that it is immutable, and two 
	/// instances with identical property values are considered equal.
	/// </remarks>
	internal class DataRecordFieldInfo
	{
		private readonly int _index;
		private readonly string _fieldName;
		private readonly Type _fieldType;

		public DataRecordFieldInfo(int index, string fieldName, Type fieldType)
		{
			_index = index;
			_fieldName = Verify.ArgumentNotNull(fieldName, "fieldName");
			_fieldType = Verify.ArgumentNotNull(fieldType, "fieldType");
		}

		public int Index
		{
			get { return _index; }
		}

		public string FieldName
		{
			get { return _fieldName; }
		}

		public Type FieldType
		{
			get { return _fieldType; }
		}

		public override bool Equals(object obj)
		{
			var other = obj as DataRecordFieldInfo;
			if (other == null)
			{
				return false;
			}
			return Index == other.Index
			       && FieldName == other.FieldName
			       && FieldType == other.FieldType;
		}

		public override int GetHashCode()
		{
			return (FieldName.GetHashCode() << 16) ^ FieldType.GetHashCode();
		}

		public override string ToString()
		{
			var sb = new StringBuilder(FieldName);
			sb.Append(" (");
			sb.Append(FieldType.Name);
			sb.Append(')');
			return sb.ToString();
		}
	}
}