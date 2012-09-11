using System;
using NHibernate.Cfg;

namespace Quokka.NH.Startup
{
	/// <summary>
	/// Interface implemented by classes that can serialize and deserialize an NHibernate <see cref="Configuration"/>
	/// to permanent storage.
	/// </summary>
	/// <remarks>
	/// Implement this interface if you wish to store <see cref="Configuration"/> objects between invocations
	/// of your program. Deserializing a <see cref="Configuration"/> can be much quicker than creating from
	/// scratch.
	/// </remarks>
	public interface IConfigurationSerializer
	{
		/// <summary>
		/// Identifies whether this serializer can serialize a <see cref="Configuration"/> for the
		/// specified database alias.
		/// </summary>
		bool CanSerialize(string alias);

		/// <summary>
		/// Attempt to deserialize the <see cref="Configuration"/> associated with the specified
		/// database alias. If no <see cref="Configuration"/> is available, returns <c>null</c>.
		/// </summary>
		Configuration Deserialize(string alias);

		/// <summary>
		/// Attempt to serialize the <see cref="Configuration"/> associated with the specified
		/// database alias to permanent storage.
		/// </summary>
		void Serialize(string alias, Configuration configuration);
	}
}
