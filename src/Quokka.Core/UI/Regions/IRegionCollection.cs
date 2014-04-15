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

using System.Collections.Generic;

namespace Quokka.UI.Regions
{
	/// <summary>
	/// Defines a collection of <see cref="IRegion"/> uniquely identified by their Name.
	/// </summary>
	/// <remarks>
	/// This interface is a straight copy from Prism
	/// </remarks>
	public interface IRegionCollection : IEnumerable<IRegion>
	{
		/// <summary>
		/// Gets the IRegion with the name received as index.
		/// </summary>
		/// <param name="regionName">Name of the region to be retrieved.</param>
		/// <returns>The <see cref="IRegion"/> identified with the requested name.</returns>
		IRegion this[string regionName] { get; }

		/// <summary>
		/// Adds a <see cref="IRegion"/> to the collection.
		/// </summary>
		/// <param name="region">Region to be added to the collection.</param>
		void Add(IRegion region);

		/// <summary>
		/// Removes a <see cref="IRegion"/> from the collection.
		/// </summary>
		/// <param name="regionName">Name of the region to be removed.</param>
		/// <returns><see langword="true"/> if the region was removed from the collection, otherwise <see langword="false"/>.</returns>
		bool Remove(string regionName);

		/// <summary>
		/// Checks if the collection contains a <see cref="IRegion"/> with the name received as parameter.
		/// </summary>
		/// <param name="regionName">The name of the region to look for.</param>
		/// <returns><see langword="true"/> if the region is contained in the collection, otherwise <see langword="false"/>.</returns>
		bool ContainsRegionWithName(string regionName);
	}
}