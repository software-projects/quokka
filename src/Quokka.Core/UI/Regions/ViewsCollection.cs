using System.Collections;
using System.Collections.Generic;

namespace Quokka.UI.Regions
{
	/// <summary>
	/// Collection of views.
	/// </summary>
	/// <remarks>
	/// TODO: Implement binding list behaviour
	/// </remarks>
	public class ViewsCollection : IViewsCollection
	{
		private readonly List<object> _innerList = new List<object>();

		public IEnumerator<object> GetEnumerator()
		{
			return _innerList.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public bool Contains(object value)
		{
			return _innerList.Contains(value);
		}

		public void Add(object value)
		{
			_innerList.Add(value);
		}

		public void Remove(object value)
		{
			_innerList.Remove(value);
		}
	}
}