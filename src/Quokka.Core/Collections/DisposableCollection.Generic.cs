#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
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

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Castle.Core.Logging;
using Quokka.Diagnostics;

namespace Quokka.Collections
{
	/// <summary>
	/// 	A collection of <see cref = "IDisposable" /> objects.
	/// </summary>
	/// <typeparam name = "T">Disposable object type</typeparam>
	/// <remarks>
	/// 	This collection implements the <see cref = "IComponent" /> interface
	/// 	for the sole purpose that in Windows Forms, it can be convenient
	/// 	to add this to the components collection.
	/// 	<example>
	/// 		<code>
	/// 			_disposables = new DisposableCollection();
	/// 			components.Add(_disposables);
	/// 		</code>
	/// 	</example>
	/// </remarks>
	public class DisposableCollection<T> : Collection<T>, IDisposableCollection<T>
		where T : IDisposable
	{
		// ReSharper disable StaticFieldInGenericType
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		// ReSharper restore StaticFieldInGenericType

		public void Dispose()
		{
			foreach (IDisposable item in this)
			{
				if (item != null)
				{
					try
					{
						item.Dispose();
					}
					catch (ObjectDisposedException)
					{
						// Many objects will throw an ObjectDisposedException if they are already
						// disposed. This is not correct. An object should only throw this if another
						// method is called after disposed. Disposed can be called as many times as
						// you like according to the CLR standard.
						Log.WarnFormat("Object of type {0} threw ObjectDisposedException during Dispose",
						               item.GetType().FullName);
					}
				}
			}
		}
	}
}