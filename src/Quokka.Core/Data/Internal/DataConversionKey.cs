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
using Quokka.Diagnostics;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// 	Tuple class used as a key to dictionary collections keyed on the combination of type of data record field
	/// 	and type of property.
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