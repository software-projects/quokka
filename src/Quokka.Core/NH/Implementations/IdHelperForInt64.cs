using System;
using Quokka.NH.Implementations;

namespace Quokka.NH 
{
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
}
