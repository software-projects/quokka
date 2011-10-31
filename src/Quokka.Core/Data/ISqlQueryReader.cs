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

namespace Quokka.Data
{
	/// <summary>
	/// 	Provides a mechanism for reading a large number of rows from the
	/// 	query without creating a large number of record objects.
	/// </summary>
	/// <typeparam name = "T">
	/// 	Type of record object returned from the <see cref = "SqlQuery" />
	/// 	query.
	/// </typeparam>
	public interface ISqlQueryReader<T> : IDisposable where T : class
	{
		/// <summary>
		/// 	Contains the data from the current row being processed by the query.
		/// </summary>
		/// <remarks>
		/// 	Before the first call to <see cref = "Read" /> is called, each property
		/// 	in this record object will contain default values.
		/// </remarks>
		T Record { get; }

		/// <summary>
		/// 	Read the next row from the query results and populate the <see cref = "Record" />
		/// 	property with the data from that row.
		/// </summary>
		/// <returns>
		/// 	Returns <c>true</c> if data from the next row was retrieved, or <c>false</c> if
		/// 	there were no more rows to process from the query results.
		/// </returns>
		bool Read();
	}
}