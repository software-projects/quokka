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
using NHibernate.Cfg;

namespace Quokka.NH.Interfaces
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
