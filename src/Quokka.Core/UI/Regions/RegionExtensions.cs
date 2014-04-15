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
using Quokka.ServiceLocation;

namespace Quokka.UI.Regions
{
	/// <summary>
	/// Extension methods for Regions.
	/// </summary>
	public static class RegionExtensions
	{
		public static bool ContainsType(this IRegion region, Type type)
		{
			return region.FindType(type) != null;
		}

		public static bool ContainsType<T>(this IRegion region)
		{
			return region.ContainsType(typeof (T));
		}

		public static object FindType(this IRegion region, Type type)
		{
			foreach (object view in region.Views)
			{
				if (type.IsAssignableFrom(view.GetType()))
				{
					return view;
				}
			}
			return null;
		}

		public static T FindType<T>(this IRegion region)
		{
			return (T) region.FindType(typeof (T));
		}

		public static void AddOrActivate(this IRegion region, Type type)
		{
			object view = region.FindType(type);
			if (view == null)
			{
				view = ServiceLocator.Current.GetInstance(type);
				region.Add(view);
			}
			region.Activate(view);
		}

		public static void AddOrActivate<T>(this IRegion region) where T : class
		{
			region.AddOrActivate(typeof (T));
		}
	}
}