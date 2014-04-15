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

using System;
using System.Collections.Generic;

namespace Quokka.NH.Implementations
{
	/// <summary>
	/// Simple class for storing objects by a string alias, which may be <c>null</c>.
	/// </summary>
	public class AliasDictionary<T> where T : class
	{
		private readonly Dictionary<string, T> _dict = new Dictionary<string, T>();
		private T _nullAliasValue;

		public T Find(string alias)
		{
			T result;
			if (alias == null)
			{
				result = _nullAliasValue;
			}
			else
			{
				_dict.TryGetValue(alias, out result);
			}
			return result;
		}

		public void Save(string alias, T value)
		{
			if (alias == null)
			{
				_nullAliasValue = value;
			}
			else
			{
				_dict[alias] = value;
			}
		}
	}
}
