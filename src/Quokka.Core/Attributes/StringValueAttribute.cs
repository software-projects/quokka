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

namespace Quokka.Attributes
{
	/// <summary>
	/// Specify a string value to associate with an enumerated value.
	/// </summary>
	/// <remarks>
	/// Use this attribute when you wish to associate a string value with an
	/// enumerated value
	/// </remarks>
	/// <example>
	/// <code>
	/// enum Sex {
	///   [StringValue("F")] Female,
	///   [StringValue("M")] Male
	/// }
	/// </code>
	/// </example>
	/// <example>
	/// The following example shows an enumerated type where one of the 
	/// values corresponds to a null value. Typically, if an enum maps
	/// to a null string, then this should be the default value for the enum type.
	/// <code>
	/// enum Answer {
	///   [StringValue(null)] NotAnswered,
	///   [StringValue("Y")] Yes,
	///   [StringValue("N")] No,
	///   [StringValue("U")] Unsure,
	/// }
	/// </code>
	/// </example>
	[AttributeUsage(AttributeTargets.Field)]
	public class StringValueAttribute : Attribute
	{
		private readonly string _stringValue;

		public StringValueAttribute(string stringValue)
		{
			_stringValue = stringValue;
		}

		public string StringValue
		{
			get { return _stringValue; }
		}
	}
}
