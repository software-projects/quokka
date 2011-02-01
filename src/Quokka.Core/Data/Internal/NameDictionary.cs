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
using System.Collections;
using System.Collections.Generic;

namespace Quokka.Data.Internal
{
	/// <summary>
	/// A collection class that is case insensitive, but will prefer a case-sensitive match if one exists.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class NameDictionary<T> : IEnumerable<KeyValuePair<string, T>>
	{
		private readonly IDictionary<string, T> _mapCaseSensitive = new Dictionary<string, T>();

		private readonly IDictionary<string, T> _mapIgnoreCase =
			new Dictionary<string, T>(StringComparer.InvariantCultureIgnoreCase);

		public int Count
		{
			get { return _mapIgnoreCase.Count; }
		}

		public T this[string key]
		{
			get
			{
				T value;
				if (!TryGetValue(key, out value))
				{
					throw new KeyNotFoundException();
				}
				return value;
			}
		}

		#region IEnumerable<KeyValuePair<string,T>> Members

		public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
		{
			return _mapCaseSensitive.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return _mapCaseSensitive.GetEnumerator();
		}

		#endregion

		public void Clear()
		{
			_mapIgnoreCase.Clear();
			_mapCaseSensitive.Clear();
		}

		public bool Contains(KeyValuePair<string, T> item)
		{
			return _mapCaseSensitive.Contains(item) || _mapIgnoreCase.Contains(item);
		}

		public bool ContainsKey(string key)
		{
			return _mapIgnoreCase.ContainsKey(key);
		}

		public void Add(string key, T value)
		{
			_mapIgnoreCase.Add(key, value);
			_mapCaseSensitive.Add(key, value);
		}

		public bool TryGetValue(string key, out T value)
		{
			return _mapCaseSensitive.TryGetValue(key, out value) || _mapIgnoreCase.TryGetValue(key, out value);
		}
	}
}