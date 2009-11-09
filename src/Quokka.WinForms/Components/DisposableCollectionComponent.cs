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
			_inner.Remove(item);
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