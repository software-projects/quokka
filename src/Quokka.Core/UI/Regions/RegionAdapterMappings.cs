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
using System.Collections.Generic;

namespace Quokka.UI.Regions
{
	/// <summary>
	/// This class maps <see cref="Type"/> with <see cref="IRegionAdapter"/>.
	/// </summary>
	/// <remarks>
	/// Adapted from Microsoft prism
	/// </remarks>
	public class RegionAdapterMappings
	{
		private readonly Dictionary<Type, IRegionAdapter> mappings = new Dictionary<Type, IRegionAdapter>();

		/// <summary>
		/// Registers the mapping between a type and an adapter.
		/// </summary>
		/// <param name="controlType">The type of the control.</param>
		/// <param name="adapter">The adapter to use with the <paramref name="controlType"/> type.</param>
		/// <exception cref="ArgumentNullException">When any of <paramref name="controlType"/> or <paramref name="adapter"/> are <see langword="null" />.</exception>
		/// <exception cref="InvalidOperationException">If a mapping for <paramref name="controlType"/> already exists.</exception>
		public void RegisterMapping(Type controlType, IRegionAdapter adapter)
		{
			if (controlType == null)
			{
				throw new ArgumentNullException("controlType");
			}

			if (adapter == null)
			{
				throw new ArgumentNullException("adapter");
			}

			if (mappings.ContainsKey(controlType))
			{
				throw new InvalidOperationException(string.Format("Mapping already exists for {0}", controlType.Name));
			}

			mappings.Add(controlType, adapter);
		}

		/// <summary>
		/// Returns the adapter associated with the type provided.
		/// </summary>
		/// <param name="controlType">The type to obtain the <seealso cref="IRegionAdapter"/> mapped.</param>
		/// <returns>The <seealso cref="IRegionAdapter"/> mapped to the <paramref name="controlType"/>.</returns>
		/// <remarks>This class will look for a registered type for <paramref name="controlType"/> and if there is not any,
		/// it will look for a registered type for any of its ancestors in the class hierarchy.
		/// If there is no registered type for <paramref name="controlType"/> or any of its ancestors,
		/// an exception will be thrown.</remarks>
		/// <exception cref="KeyNotFoundException">When there is no registered type for <paramref name="controlType"/> or any of its ancestors.</exception>
		public IRegionAdapter GetMapping(Type controlType)
		{
			Type currentType = controlType;

			while (currentType != null)
			{
				if (mappings.ContainsKey(currentType))
				{
					return mappings[currentType];
				}
				currentType = currentType.BaseType;
			}
			throw new KeyNotFoundException("controlType");
		}
	}
}