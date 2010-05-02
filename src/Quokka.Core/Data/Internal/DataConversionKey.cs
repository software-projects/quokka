using System;
using Quokka.Diagnostics;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// Tuple class used as a key to dictionary collections keyed on the combination of type of data record field
	/// and type of property.
	/// </summary>
	internal class DataConversionKey
	{
		public Type FieldType { get; private set; }
		public Type PropertyType { get; private set; }

		public DataConversionKey(Type fieldType, Type propertyType)
		{
			FieldType = Verify.ArgumentNotNull(fieldType, "fieldType");
			PropertyType = Verify.ArgumentNotNull(propertyType, "propertyType");
		}

		public override bool Equals(object obj)
		{
			var other = obj as DataConversionKey;
			if (other == null)
			{
				return false;
			}

			return other.FieldType == FieldType && other.PropertyType == PropertyType;
		}

		public override int GetHashCode()
		{
			return (FieldType.GetHashCode() << 16) ^ PropertyType.GetHashCode();
		}
	}
}