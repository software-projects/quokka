﻿#region License

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
using System.Windows.Forms;
using Quokka.Diagnostics;

namespace Quokka.WinForms
{
	public interface IVirtualDataSource<TItem> where TItem : class
	{
		event EventHandler ListChanged;
		bool BuildListRequired { get; }
		int Count { get; }
		Predicate<TItem> Filter { get; set; }
		Comparison<TItem> Comparer { get; }
		SortOrder SortOrder { get; }
		void SetComparer(Comparison<TItem> comparer);
		void Clear();
		TItem GetAt(int index);
		void Sort();
		void ForEach(Action<TItem> action);
	}

	/// <summary>
	/// 	Used as an in-memory store useful for a store against lists and data grids that use virtual mode.
	/// </summary>
	/// <typeparam name = "T">The view model object in the store</typeparam>
	public class VirtualDataSource<T> : VirtualDataSource<int, T> where T : class
	{
		/// <summary>
		/// Default constructor only available for sub-classes, which should override the <see cref="GetId"/> method.
		/// </summary>
		protected VirtualDataSource() { }

		public VirtualDataSource(Func<T, int> getIdCallback) : base(getIdCallback) {}
	}

	/// <summary>
	/// 	Used as an in-memory store useful for a store against lists and data grids that use virtual mode.
	/// </summary>
	/// <typeparam name = "TId">The type of unique identifier used for type <typeparamref name = "TItem" /></typeparam>
	/// <typeparam name = "TItem">The view model object in the store</typeparam>
	public class VirtualDataSource<TId, TItem> : IVirtualDataSource<TItem> where TItem : class
	{
		private static readonly Predicate<TItem> DefaultFilter = (t => true);
		private readonly Dictionary<TId, TItem> _dict = new Dictionary<TId, TItem>();
		private readonly List<TItem> _list = new List<TItem>();
		private readonly object _lockObject = new object();
		private Comparison<TItem> _comparer;
		private SortOrder _sortOrder;
		private Predicate<TItem> _filter = DefaultFilter;
		private bool _sortRequired = true;
		private bool _buildListRequired = true;
		private readonly Func<TItem, TId> _getIdCallback; 

		public event EventHandler ListChanged;

		/// <summary>
		/// Default constructor only available for sub-classes, which should override the <see cref="GetId"/> method.
		/// </summary>
		protected VirtualDataSource() {}

		public VirtualDataSource(Func<TItem, TId> getIdCallback)
		{
			_getIdCallback = Verify.ArgumentNotNull(getIdCallback, "getIdCallback");
		}

		public bool BuildListRequired
		{
			get { return _buildListRequired || _sortRequired; }
		}

		/// <summary>
		/// 	Count of objects currently visible in the store.
		/// </summary>
		public int Count
		{
			get
			{
				lock (_lockObject)
				{
					if (_buildListRequired)
					{
						BuildList();
					}
					return _list.Count;
				}
			}
		}

		/// <summary>
		/// 	Predicate used to define what is visible in the store, and what is not.
		/// </summary>
		public Predicate<TItem> Filter
		{
			get { return _filter; }
			set
			{
				_filter = value ?? DefaultFilter;
				_buildListRequired = true;
				_sortRequired = true;
				RaiseListChanged();
			}
		}

		/// <summary>
		/// 	Comparer used to sort items in the store.
		/// </summary>
		public Comparison<TItem> Comparer
		{
			get { return _comparer; }
		}

		public SortOrder SortOrder
		{
			get { return _sortOrder; }
		}

		public void SetComparer(Comparison<TItem> comparer)
		{
			if (_comparer == comparer)
			{
				_sortOrder = _sortOrder == SortOrder.Ascending ? SortOrder.Descending : SortOrder.Ascending;
			}
			else
			{
				_comparer = comparer;
				_sortOrder = _comparer == null ? SortOrder.None : SortOrder.Ascending;
			}
			_sortRequired = true;
			RaiseListChanged();
		}

		/// <summary>
		/// 	Clear all objects from the store.
		/// </summary>
		public void Clear()
		{
			lock (_lockObject)
			{
				_dict.Clear();
				_list.Clear();
				_buildListRequired = true;
				_sortRequired = true;
			}
			RaiseListChanged();
		}

		/// <summary>
		/// 	Get the item at the specified index in the store.
		/// </summary>
		/// <param name = "index">Index to use</param>
		/// <returns>
		/// 	Object at the specified index, or <c>null</c> if the index is out of range (as can sometimes
		/// 	happen with virtual mode mapping).
		/// </returns>
		public TItem GetAt(int index)
		{
			lock (_lockObject)
			{
				if (_buildListRequired)
				{
					BuildList();
				}
				if (_sortRequired)
				{
					Sort();
				}
				if (index < 0 || index >= _list.Count)
				{
					return null;
				}
				return _list[index];
			}
		}

		/// <summary>
		/// 	Creates a <see cref = "List{TItem}" /> with the current contents of the store.
		/// </summary>
		/// <returns>A copied list.</returns>
		public List<TItem> ToList()
		{
			lock (_lockObject)
			{
				if (_buildListRequired)
				{
					BuildList();
				}
				if (_sortRequired)
				{
					Sort();
				}
				return new List<TItem>(_list);
			}
		}

		/// <summary>
		/// 	Add an item to the store. Depending on whether the item matches the <see cref = "Filter" /> predicate,
		/// 	this item may or may not become visible.
		/// </summary>
		/// <param name = "obj"></param>
		public void Add(TItem obj)
		{
			// ignore null objects
			if (obj == null)
			{
				return;
			}

			bool raiseListChanged = false;

			lock (_lockObject)
			{
				TId id = GetId(obj);
				_dict[id] = obj;
				if (_filter(obj))
				{
					// visible according to the current filter, so we have to rebuild and resort
					_sortRequired = true;
					_buildListRequired = true;
					raiseListChanged = true;
				}
			}

			if (raiseListChanged)
			{
				RaiseListChanged();
			}
		}

		/// <summary>
		/// 	Add or update an item to the store. If the item does not already exist in the store, it
		///		is added. If there is an item in the store with the same Id, then it is replaced.
		/// </summary>
		/// <param name = "obj"></param>
		/// <remarks>
		///		Unlike the <see cref="Add"/> method, this method will force a rebuild of the list
		///		regardless of whether the updated item is visible according to the current filter.
		///		This is because the update may have caused the object to go from visible to invisible,
		///		or vice-versa.
		/// </remarks>
		public void Update(TItem obj)
		{
			// ignore null objects
			if (obj == null)
			{
				return;
			}

			lock (_lockObject)
			{
				TId id = GetId(obj);
				_dict[id] = obj;

				_sortRequired = true;
				_buildListRequired = true;
			}

			RaiseListChanged();
		}

		/// <summary>
		/// 	Add a range of items to the store. This is considerably more efficent than calling <see cref = "Add" />
		/// 	repeatedly for each item in a large collection.
		/// </summary>
		/// <param name = "list">Collection of items</param>
		public void AddRange(IEnumerable<TItem> list)
		{
			if (list == null)
			{
				return;
			}
			lock (_lockObject)
			{
				foreach (TItem obj in list)
				{
					if (obj == null)
					{
						continue;
					}
					TId id = GetId(obj);
					_dict[id] = obj;
				}
				_sortRequired = true;
				_buildListRequired = true;
			}
			RaiseListChanged();
		}

		public void ReplaceContents(IEnumerable<TItem> list)
		{
			lock (_lockObject)
			{
				_dict.Clear();
				_list.Clear();
				if (list != null)
				{
					foreach (TItem obj in list)
					{
						if (obj == null)
						{
							continue;
						}
						TId id = GetId(obj);
						_dict[id] = obj;
					}
				}
				_sortRequired = true;
				_buildListRequired = true;
			}
			RaiseListChanged();
		}

		/// <summary>
		/// 	Find the index of an item in the store.
		/// </summary>
		/// <param name = "obj">Object to find index of.</param>
		/// <returns>
		/// 	Index of item in the store, or -1 if the item is not in the store, or is not visible in the store.
		/// </returns>
		public int FindIndex(TItem obj)
		{
			if (obj == null)
			{
				return -1;
			}

			lock (_lockObject)
			{
				if (_buildListRequired)
				{
					BuildList();
				}
				if (_sortRequired)
				{
					Sort();
				}

				TId id = GetId(obj);

				return _list.FindIndex(o => GetId(o).Equals(id));
			}
		}

		private int DescendingComparer(TItem a, TItem b)
		{
			return -_comparer(a, b);
		}

		/// <summary>
		/// 	Sorts the store according to the current <see cref = "Comparer" />
		/// </summary>
		public void Sort()
		{
			if (_comparer != null)
			{
				lock (_lockObject)
				{
					if (_buildListRequired)
					{
						BuildList();
					}

					if (_sortOrder == SortOrder.Descending)
					{
						_list.Sort(DescendingComparer);
					}
					else
					{
						_list.Sort(_comparer);
					}
				}
			}
			_sortRequired = false;
		}

		/// <summary>
		/// 	Performs the specified action on each visible item in the store.
		/// </summary>
		/// <param name = "action">Action to apply to each item</param>
		public void ForEach(Action<TItem> action)
		{
			lock (_lockObject)
			{
				if (_buildListRequired)
				{
					BuildList();
				}
				if (_sortRequired)
				{
					Sort();
				}

				_list.ForEach(action);
			}
		}

		public List<TItem> FindAll(Predicate<TItem> condition)
		{
			lock (_lockObject)
			{
				if (_buildListRequired)
				{
					BuildList();
				}
				if (_sortRequired)
				{
					Sort();
				}

				return _list.FindAll(condition);
			}
		}

		public void Invalidate()
		{
			lock (_lockObject)
			{
				_buildListRequired = true;
				_sortRequired = true;
			}
			RaiseListChanged();
		}

		public TItem FindById(TId id)
		{
			lock (_lockObject)
			{
				TItem obj;
				_dict.TryGetValue(id, out obj);
				return obj;
			}
		}

		public bool Remove(TId id)
		{
			bool result;
			lock (_lockObject)
			{
				TItem obj;
				result = _dict.TryGetValue(id, out obj);
				if (result)
				{
					_dict.Remove(id);
					_list.Remove(obj);
					// no need to rebuild list or sort, because
					// only one item was removed
				}
			}
			if (result)
			{
				RaiseListChanged();
			}
			return result;
		}

		/// <summary>
		/// 	Implemented by the derived class. Provides a unique identifier that describes the item.
		/// </summary>
		/// <param name = "obj">Item in the store, or about to be added to the store.</param>
		/// <returns>Unique value across all items.</returns>
		protected virtual TId GetId(TItem obj)
		{
			if (_getIdCallback == null)
			{
				throw new InvalidOperationException(
					"Need to specify getIdCallback, or derive from this class and override GetId() method");
			}
			return _getIdCallback(obj);
		}

		private void BuildList()
		{
			_list.Clear();
			foreach (TItem obj in _dict.Values)
			{
				if (_filter(obj))
				{
					_list.Add(obj);
				}
			}
			_buildListRequired = false;
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