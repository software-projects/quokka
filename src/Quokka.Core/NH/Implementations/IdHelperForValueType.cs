using System;

namespace Quokka.NH.Implementations
{
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
}
