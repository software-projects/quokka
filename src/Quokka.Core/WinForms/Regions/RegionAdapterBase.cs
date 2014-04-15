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
using Quokka.Diagnostics;
using Quokka.UI.Regions;

namespace Quokka.WinForms.Regions
{
	/// <summary>
	/// Base class to facilitate the creation of <see cref="IRegionAdapter"/> implementations.
	/// </summary>
	/// <typeparam name="T">Type of object to adapt.</typeparam>
	public abstract class RegionAdapterBase<T> : IRegionAdapter where T : class
	{
		/// <summary>
		/// Adapts an object and binds it to a new <see cref="IRegion"/>.
		/// </summary>
		/// <param name="regionTarget">The object to adapt.</param>
		/// <param name="regionName">The name of the region to be created.</param>
		/// <returns>The new instance of <see cref="IRegion"/> that the <paramref name="regionTarget"/> is bound to.</returns>
		public IRegion Initialize(T regionTarget, string regionName)
		{
			Verify.ArgumentNotNull(regionTarget, "regionTarget");
			Verify.ArgumentNotNull(regionName, "regionName");

			IRegion region = CreateRegion();
			region.Name = regionName;

			Adapt(region, regionTarget);
			AttachBehaviors(region, regionTarget);
			AttachDefaultBehaviors(region, regionTarget);
			return region;
		}

		/// <summary>
		/// Adapts an object and binds it to a new <see cref="IRegion"/>.
		/// </summary>
		/// <param name="regionTarget">The object to adapt.</param>
		/// <param name="regionName">The name of the region to be created.</param>
		/// <returns>The new instance of <see cref="IRegion"/> that the <paramref name="regionTarget"/> is bound to.</returns>
		/// <remarks>This methods performs validation to check that <paramref name="regionTarget"/>
		/// is of type <typeparamref name="T"/>.</remarks>
		/// <exception cref="ArgumentNullException">When <paramref name="regionTarget"/> is <see langword="null" />.</exception>
		/// <exception cref="InvalidOperationException">When <paramref name="regionTarget"/> is not of type <typeparamref name="T"/>.</exception>
		IRegion IRegionAdapter.Initialize(object regionTarget, string regionName)
		{
			return Initialize(GetCastedObject(regionTarget), regionName);
		}

		/// <summary>
		/// This method adds the default behaviors by using the <see cref="IRegionBehaviorFactory"/> object.
		/// </summary>
		/// <param name="region">The region being used.</param>
		/// <param name="regionTarget">The object to adapt.</param>
		protected virtual void AttachDefaultBehaviors(IRegion region, T regionTarget)
		{
		}

		/// <summary>
		/// Template method to attach new behaviors.
		/// </summary>
		/// <param name="region">The region being used.</param>
		/// <param name="regionTarget">The object to adapt.</param>
		protected virtual void AttachBehaviors(IRegion region, T regionTarget)
		{
		}

		/// <summary>
		/// Template method to adapt the object to an <see cref="IRegion"/>.
		/// </summary>
		/// <param name="region">The new region being used.</param>
		/// <param name="regionTarget">The object to adapt.</param>
		protected abstract void Adapt(IRegion region, T regionTarget);

		/// <summary>
		/// Template method to create a new instance of <see cref="IRegion"/>
		/// that will be used to adapt the object.
		/// </summary>
		/// <returns>A new instance of <see cref="IRegion"/>.</returns>
		protected abstract IRegion CreateRegion();

		private static T GetCastedObject(object regionTarget)
		{
			Verify.ArgumentNotNull(regionTarget, "regionTarget");

			T castedObject = regionTarget as T;
			if (castedObject == null)
			{
				throw new InvalidOperationException("Failed to cast object");
			}

			return castedObject;
		}
	}
}