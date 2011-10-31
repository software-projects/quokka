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
using System.Data;
using System.Text;
using Quokka.Diagnostics;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// 	Contains information about a field in an <see cref = "IDataRecord" />.
	/// </summary>
	/// <remarks>
	/// 	This type can be used as a value type in the sense that it is immutable, and two 
	/// 	instances with identical property values are considered equal.
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