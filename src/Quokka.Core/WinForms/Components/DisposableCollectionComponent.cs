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
using System.ComponentModel;
using Quokka.Collections;

namespace Quokka.WinForms.Components
{
	/// <summary>
	/// A collection of <see cref="IDisposable"/> objects that can be conveniently added to a windows form control as a component.
	/// </summary>
	public class DisposableCollectionComponent : Component, IDisposableCollection
	{
		private readonly DisposableCollection _inner = new DisposableCollection();

		public DisposableCollectionComponent() : this(null)
		{
		}

		public DisposableCollectionComponent(IContainer container)
		{
			if (container != null)
			{
				container.Add(this);
			}
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (disposing)
			{
				_inner.Dispose();
			}
		}

		public IEnumerator<IDisposable> GetEnumerator()
		{
			return _inner.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(IDisposable item)
		{
			_inner.Add(item);
		}

		public void Clear()
		{
			_inner.Clear();
		}

		public bool Contains(IDisposable item)
		{
			return _inner.Contains(item);
		}

		public void CopyTo(IDisposable[] array, int arrayIndex)
		{
			_inner.CopyTo(array, arrayIndex);
		}

		public bool Remove(IDisposable item)
		{
			return _inner.Remove(item);
		}

		public int Count
		{
			get { return _inner.Count; }
		}

		public bool IsReadOnly
		{
			get { return ((ICollection<IDisposable>) _inner).IsReadOnly; }
		}
	}
}