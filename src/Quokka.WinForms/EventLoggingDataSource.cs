using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Quokka.WinForms
{
	/// <summary>
	/// A virtual data source optimised for displaying larger event log displays. It
	/// does not provide any sorting functionality.
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EventLoggingDataSource<T> : IVirtualDataSource<T> where T : class
	{
		private const int MaxCapacity = 1300;
		private const int PreferredCapacity = 1000;

		// list of all event log entries
		private readonly List<T> _list = new List<T>();

		public event EventHandler ListChanged;

		public bool BuildListRequired
		{
			// This is for indicating that a lengthy build list is
			// required after changing sort order. Not relevant here.
			get { return false; }
		}

		public int Count
		{
			get { return _list.Count; }
		}

		// Filter not implemented yet.
		public Predicate<T> Filter { get; set; }

		private Comparison<T> _comparer;
		private SortOrder _sortOrder;

		public Comparison<T> Comparer
		{
			get { return _comparer; }
		}

		public SortOrder SortOrder
		{
			get { return _sortOrder; }
		}

		// sorting not implemented
		public void SetComparer(Comparison<T> comparer)
		{
			_comparer = comparer;
			_sortOrder = SortOrder.None;
		}

		public void Clear()
		{
			_list.Clear();
		}

		public T GetAt(int index)
		{
			if (index >= _list.Count)
			{
				return null;
			}
			return _list[index];
		}

		public void Sort()
		{
		}

		public void ForEach(Action<T> action)
		{
			_list.ForEach(action);
		}

		public void Add(T item)
		{
			// Because removing items from the front of the array is an O(N) operation
			// we only do it when the list gets to its maximum threshold. Then we remove
			// a decent portion of the list in one go.
			if (_list.Count >= MaxCapacity)
			{
				_list.RemoveRange(0, _list.Count - PreferredCapacity);
			}

			_list.Add(item);
			RaiseListChanged();
		}

		private void RaiseListChanged()
		{
			if (ListChanged != null)
			{
				ListChanged(this, EventArgs.Empty);
			}
		}
	}
}