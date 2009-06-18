using System.Collections.Generic;

namespace Quokka.UI.Regions
{
	/// <summary>
	/// Defines a collection of view objects.
	/// </summary>
	public interface IViewsCollection : IEnumerable<object>
	{
		/// <summary>
		/// Determines whether the collection contains a specific value.
		/// </summary>
		/// <param name="value">The object to locate in the collection.</param>
		/// <returns>
		/// <see langword="true" /> if <paramref name="value"/> is found in the collection; 
		/// otherwise, <see langword="false" />.
		/// </returns>
		bool Contains(object value);
	}
}