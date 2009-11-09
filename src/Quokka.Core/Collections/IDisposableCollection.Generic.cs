using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quokka.Collections
{
	/// <summary>
	/// A collection of objects that are disposable. The collection itself implements <see cref="IDisposable"/>
	/// </summary>
	/// <typeparam name="T">Type of objects in the collection</typeparam>
	public interface IDisposableCollection<T> : IDisposable, ICollection<T> where T : IDisposable
	{

	}
}
