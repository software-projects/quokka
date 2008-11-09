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
using Quokka.DomainModel;
using Quokka.PresentationModel.Internal;

namespace Quokka.PresentationModel
{
	public class PresBindingList<TPres, TDomain> : SortableBindingList<TPres>
		where TDomain : DomainObject<TDomain>
		where TPres : PresentationObject<TDomain>, new()
	{
		private readonly Dictionary<int, TPres> _dict = new Dictionary<int, TPres>();

		protected override void ClearItems()
		{
			base.ClearItems();
			_dict.Clear();
		}

		protected override void InsertItem(int index, TPres item)
		{
			base.InsertItem(index, item);
			_dict[item.Id] = item;
		}

		protected override void RemoveItem(int index)
		{
			TPres item = base[index];
			base.RemoveItem(index);
			_dict.Remove(item.Id);
		}

		protected override void SetItem(int index, TPres item)
		{
			TPres prevItem = base[index];
			base.SetItem(index, item);
			_dict[item.Id] = item;
			if (prevItem.Id != item.Id)
			{
				_dict.Remove(prevItem.Id);
			}
		}

		public TPres FindById(int id)
		{
			TPres item;
			_dict.TryGetValue(id, out item);
			return item;
		}

		public int FindPosition(TPres pres)
		{
			int index = 0;
			foreach (TPres p in this)
			{
				if (p == pres)
					return index;
				index++;
			}
			return -1;
		}

		/// <summary>
		/// Use this method instead of <see cref="UpdateFrom"/> when you are completely replacing the
		/// contents of a list instead of performing a refresh. It is slightly quicker than 
		/// <see cref="UpdateFrom"/>.
		/// </summary>
		/// <param name="domainObjects"></param>
		public void ReplaceContents(IEnumerable<TDomain> domainObjects)
		{
			RaiseListChangedEvents = false;
			try
			{
				Clear();
				foreach (TDomain domainObject in domainObjects)
				{
					TPres presObject = new TPres();
					presObject.LoadFrom(domainObject);
					Add(presObject);
				}
			}
			finally
			{
				RaiseListChangedEvents = true;
				RaiseListReset();
			}
		}

		private void RaiseListReset()
		{
			ListChangedEventArgs e = new ListChangedEventArgs(ListChangedType.Reset, 0, 0);
			OnListChanged(e);
		}

		public void UpdateFrom(IEnumerable<TDomain> domainObjects)
		{
			if (domainObjects == null)
			{
				Clear();
				return;
			}

			// First, setup a dictionary of all items in the list. When all
			// the updates are done, this dictionary will contain all of the items
			// to remove from the list
			Dictionary<int, TPres> itemsToRemove = new Dictionary<int, TPres>();
			foreach (TPres pres in this)
			{
				itemsToRemove.Add(pres.Id, pres);
			}

			// update each item in the binding list, adding new items as necessary
			foreach (TDomain domain in domainObjects)
			{
				itemsToRemove.Remove(domain.Id);
				TPres pres = FindById(domain.Id);
				if (pres == null)
				{
					pres = new TPres();
					pres.LoadFrom(domain);
					Add(pres);
				}
				else
				{
					pres.LoadFrom(domain);
				}
			}

			// remove items that need to be removed
			foreach (TPres pres in itemsToRemove.Values)
			{
				Remove(pres);
			}
		}

		/// <summary>
		/// Update the following items in the list, but do not remove items from
		/// the binding list.
		/// </summary>
		/// <param name="domainObjects"></param>
		public void UpdateFromPartialList(IEnumerable<TDomain> domainObjects)
		{
			foreach (TDomain domain in domainObjects)
			{
				TPres pres = FindById(domain.Id);
				if (pres == null)
				{
					pres = new TPres();
					pres.LoadFrom(domain);
					Add(pres);
				}
				else
				{
					pres.LoadFrom(domain);
				}
			}
		}
	}
}