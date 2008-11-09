#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2008 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Quokka.PresentationModel.Internal;

// Code adapted from sample written by Michael Weinhardt
// http://msdn2.microsoft.com/en-us/library/ms993124.aspx

namespace Quokka.PresentationModel.Internal
{
	public class SortableBindingList<T> : BindingList<T>
	{
		#region Sorting

		private bool _isSorted;
		private PropertyDescriptor _sortProperty;
		private ListSortDirection _sortDirection;

		protected override bool SupportsSortingCore
		{
			get { return true; }
		}

		// Missing from Part 2
		protected override ListSortDirection SortDirectionCore
		{
			get { return _sortDirection; }
		}

		// Missing from Part 2
		protected override PropertyDescriptor SortPropertyCore
		{
			get { return _sortProperty; }
		}

		protected virtual IComparer<T> GetComparer(PropertyDescriptor property, ListSortDirection direction)
		{
			return new PropertyComparer<T>(property, direction);
		}

		protected override void ApplySortCore(PropertyDescriptor property, ListSortDirection direction)
		{
			// Get list to sort
			// Note: this.Items is a non-sortable ICollection<T>
			List<T> items = Items as List<T>;

			// Apply and set the sort, if items to sort
			if (items != null)
			{
				IComparer<T> pc = GetComparer(property, direction);
				items.Sort(pc);
				_isSorted = true;
			}
			else
			{
				_isSorted = false;
			}

			_sortProperty = property;
			_sortDirection = direction;

			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		protected override bool IsSortedCore
		{
			get { return _isSorted; }
		}

		protected override void RemoveSortCore()
		{
			_isSorted = false;
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		#endregion

		#region Persistence

		// NOTE: BindingList<T> is not serializable but List<T> is

		public void Save(string filename)
		{
			BinaryFormatter formatter = new BinaryFormatter();
			using (FileStream stream = new FileStream(filename, FileMode.Create))
			{
				// Serialize data list items
				formatter.Serialize(stream, Items);
			}
		}

		public void Load(string filename)
		{
			ClearItems();

			if (File.Exists(filename))
			{
				BinaryFormatter formatter = new BinaryFormatter();
				using (FileStream stream = new FileStream(filename, FileMode.Open))
				{
					// Deserialize data list items
					((List<T>)Items).AddRange((IEnumerable<T>)formatter.Deserialize(stream));
				}
			}

			// Let bound controls know they should refresh their views
			OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, -1));
		}

		#endregion

		#region Searching

		protected override bool SupportsSearchingCore
		{
			get { return true; }
		}

		protected override int FindCore(PropertyDescriptor property, object key)
		{
			// Specify search columns
			if (property == null) return -1;

			// Get list to search
			List<T> items = (List<T>)Items;

			// Traverse list for value
			foreach (T item in items)
			{
				// Test column search value
				string value = (string)property.GetValue(item);

				// If value is the search value, return the 
				// index of the data item
				if ((string)key == value) return IndexOf(item);
			}
			return -1;
		}

		#endregion
	}
}