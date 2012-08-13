using System;

namespace Quokka.NH.Implementations
{
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
}
