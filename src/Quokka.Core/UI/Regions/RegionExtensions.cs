using System;
using Microsoft.Practices.ServiceLocation;

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
			else
			{
				region.Activate(view);
			}
		}

		public static void AddOrActivate<T>(this IRegion region) where T : class
		{
			region.AddOrActivate(typeof (T));
		}
	}
}