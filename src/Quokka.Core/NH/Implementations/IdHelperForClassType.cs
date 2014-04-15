#region License

// Copyright 2004-2013 John Jeffery <john@jeffery.id.au>
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

namespace Quokka.NH.Implementations
{
	/// <summary>
	/// Implementation of <see cref="IIdHelper{T}"/> that works for any class type.
	/// </summary>
	/// <typeparam name="TId">
	/// Type of entity identifier.
	/// </typeparam>
	public class IdHelperForClassType<TId> : IIdHelper<TId> where TId : class, IComparable
	{
		public bool IsDefaultValue(TId id)
		{
			return id == null;
		}

		public bool IsNull(TId id)
		{
			return id == null;
		}

		public int Compare(TId id1, TId id2)
		{
			if (id1 == null)
			{
				if (id2 == null)
				{
					return 0;
				}
				return -1;
			}
			if (id2 == null)
			{
				return 1;
			}
			return id1.CompareTo(id2);
		}

		public bool AreEqual(TId id1, TId id2)
		{
			if (id1 == null)
			{
				if (id2 == null)
				{
					return true;
				}
				return false;
			}
			return id1.Equals(id2);
		}

		public int GetHashCode(TId id)
		{
			if (id == null)
			{
				return 0;
			}
			return id.GetHashCode();
		}
	}
}
