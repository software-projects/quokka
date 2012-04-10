using System;

namespace Quokka.NH
{
	/// <summary>
	/// Interface that provides helper methods for NHibernate identity values.
	/// </summary>
	/// <typeparam name="TId">
	/// Type of entity identifer, usually <c>int</c> or <c>string</c>.
	/// </typeparam>
	public interface IIdHelper<in TId>
	{
		bool IsDefaultValue(TId id);
		bool IsNull(TId id);
		int Compare(TId id1, TId id2);
		bool AreEqual(TId id1, TId id2);
		int GetHashCode(TId id);
	}

	/// <summary>
	/// Implementation of <see cref="IIdHelper{T}"/> that works for any value type.
	/// </summary>
	/// <typeparam name="TId">
	/// Type of entity identifier.
	/// </typeparam>
	public class IdHelperForValueType<TId> : IIdHelper<TId> where TId : struct, IComparable
	{
		// ReSharper disable StaticFieldInGenericType
		private static readonly object BoxedDefaultType = default(TId);
		// ReSharper restore StaticFieldInGenericType

		public bool IsDefaultValue(TId id)
		{
			return id.Equals(BoxedDefaultType);
		}

		public bool IsNull(TId id)
		{
			return false;
		}

		public virtual int Compare(TId id1, TId id2)
		{
			return id1.CompareTo(id2);
		}

		public bool AreEqual(TId id1, TId id2)
		{
			return id1.Equals(id2);
		}

		public int GetHashCode(TId id)
		{
			var x = id.GetHashCode();
			return x;
		}
	}

	/// <summary>
	/// Implementation of <see cref="IIdHelper{T}"/> that is optimised for <see cref="Int32"/>.
	/// </summary>
	public class IdHelperForInt32 : IIdHelper<int>
	{
		public bool IsDefaultValue(int id)
		{
			return id == 0;
		}

		public bool IsNull(int id)
		{
			return false;
		}

		public virtual int Compare(int id1, int id2)
		{
			return id1 - id2;
		}

		public bool AreEqual(int id1, int id2)
		{
			return id1 == id2;
		}

		public int GetHashCode(int id)
		{
			return id.GetHashCode();
		}
	}

	/// <summary>
	/// Implementation of <see cref="IIdHelper{T}"/> that is optimised for <see cref="Int64"/>.
	/// </summary>
	public class IdHelperForInt64 : IIdHelper<long>
	{
		public bool IsDefaultValue(long id)
		{
			return id == 0;
		}

		public bool IsNull(long id)
		{
			return false;
		}

		public virtual int Compare(long id1, long id2)
		{
			return id1.CompareTo(id2);
		}

		public bool AreEqual(long id1, long id2)
		{
			return id1 == id2;
		}

		public int GetHashCode(long id)
		{
			return id.GetHashCode();
		}
	}

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

	public class IdHelperForString : IIdHelper<string>
	{
		public bool IsDefaultValue(string id)
		{
			return id == null;
		}

		public bool IsNull(string id)
		{
			return id == null;
		}

		public int Compare(string id1, string id2)
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

			// Needs to be invariant culture to work with GetHashCode
			return StringComparer.InvariantCultureIgnoreCase.Compare(id1, id2);
		}

		public bool AreEqual(string id1, string id2)
		{
			if (id1 == null)
			{
				if (id2 == null)
				{
					return true;
				}
				return false;
			}
			// Needs to be invariant culture to work with GetHashCode
			return StringComparer.InvariantCultureIgnoreCase.Compare(id1, id2) == 0;
		}

		public int GetHashCode(string id)
		{
			if (id == null)
			{
				return 0;
			}
			return id.ToUpperInvariant().GetHashCode();
		}
	}
}
