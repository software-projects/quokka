using System;

namespace Quokka.NH.Implementations
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
}
