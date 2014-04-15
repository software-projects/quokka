#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System.Collections;
using System.Collections.Generic;

namespace Quokka.UI.Regions
{
	/// <summary>
	/// Collection of views.
	/// </summary>
	/// <remarks>
	/// TODO: Implement binding list behaviour
	/// </remarks>
	public class ViewsCollection : IViewsCollection
	{
		private readonly List<object> _innerList = new List<object>();

		public IEnumerator<object> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Contains(object value)
		{
			return _innerList.Contains(value);
		}

		public void Add(object value)
		{
			_innerList.Add(value);
		}

		public void Remove(object value)
		{
			_innerList.Remove(value);
		}
	}
}