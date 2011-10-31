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

using System.Data;
using Quokka.Diagnostics;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// 	Implementation of <see cref = "ISqlQueryReader{T}" />.
	/// </summary>
	/// <typeparam name = "T"></typeparam>
	internal class QueryReader<T> : ISqlQueryReader<T> where T : class, new()
	{
		private readonly DataRecordConverter _converter;
		private readonly IDataReader _dataReader;
		private readonly T _record;

		public QueryReader(IDataReader dataReader)
		{
			_dataReader = Verify.ArgumentNotNull(dataReader, "dataReader");
			_record = new T();
			_converter = DataRecordConverter.CreateConverterFor(typeof (T), dataReader);
		}

		#region ISqlQueryReader<T> Members

		public void Dispose()
		{
			_dataReader.Dispose();
		}

		public T Record
		{
			get { return _record; }
		}

		public bool Read()
		{
			if (!_dataReader.Read())
			{
				return false;
			}

			_converter.CopyTo(_record);
			return true;
		}

		#endregion
	}
}