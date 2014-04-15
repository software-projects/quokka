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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quokka.UI.Regions;

namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// This class is responsible for maintaining a collection of regions and attaching regions to controls. 
	/// </summary>
	public class RegionManager : IRegionManager
	{
		private readonly RegionCollection _regions;

		/// <summary>
		/// Initializes a new instance of <see cref="RegionManager"/>.
		/// </summary>
		public RegionManager()
		{
			_regions = new RegionCollection(this);
		}

		/// <summary>
		/// Gets a collection of <see cref="IRegion"/> that identify each region by name. 
		/// You can use this collection to add or remove regions to the current region manager.
		/// </summary>
		/// <value>A <see cref="IRegionCollection"/> with all the registered regions.</value>
		public IRegionCollection Regions
		{
			get { return _regions; }
		}

		/// <summary>
		/// Creates a new region manager.
		/// </summary>
		/// <returns>A new region manager that can be used as a different scope from the current region manager.</returns>
		public IRegionManager CreateRegionManager()
		{
			return new RegionManager();
		}

		private class RegionCollection : IRegionCollection
		{
			private readonly IRegionManager _regionManager;
			private readonly List<IRegion> _regions;

			public RegionCollection(IRegionManager regionManager)
			{
				this._regionManager = regionManager;
				_regions = new List<IRegion>();
			}

			public IEnumerator<IRegion> GetEnumerator()
			{
				return this._regions.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}

			public IRegion this[string regionName]
			{
				get
				{
					IRegion region = GetRegionByName(regionName);
					if (region == null)
					{
						throw new KeyNotFoundException("Cannot find region: " + regionName);
					}

					return region;
				}
			}

			public void Add(IRegion region)
			{
				if (region.Name == null)
				{
					throw new InvalidOperationException("Region name cannot be empty");
				}

				if (this.GetRegionByName(region.Name) != null)
				{
					throw new ArgumentException("Region already exists: " + region.Name);
				}

				var r = region as Region;
				if (r != null)
				{
					if (r.RegionManager != null)
					{
						throw new ArgumentException("Region is already associated with a region manager");
					}
					r.RegionManager = _regionManager;

					string regionName = region.Name;
					r.RegionClosed += delegate { Remove(regionName); };
				}

				_regions.Add(region);
			}

			public bool Remove(string regionName)
			{
				bool removed = false;

				IRegion region = GetRegionByName(regionName);
				if (region != null)
				{
					removed = true;
					_regions.Remove(region);

					var r = region as Region;
					if (r != null)
					{
						r.RegionManager = null;
					}
				}

				return removed;
			}

			public bool ContainsRegionWithName(string regionName)
			{
				return GetRegionByName(regionName) != null;
			}

			private IRegion GetRegionByName(string regionName)
			{
				return _regions.FirstOrDefault(r => r.Name == regionName);
			}
		}
	}
}