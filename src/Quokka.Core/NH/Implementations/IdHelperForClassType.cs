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
