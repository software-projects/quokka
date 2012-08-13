using System;

namespace Quokka.NH.Implementations
{
	/// <summary>
	/// Implementation of <see cref="IIdHelper{T}"/> that works for a string.
	/// </summary>
	/// <remarks>
	/// TODO: String comparisons are invariant culture, case-insensitive. This works
	/// well for databases where string comparison is case-sensitive, but may not be 
	/// appropriate in all cases.
	/// </remarks>
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
