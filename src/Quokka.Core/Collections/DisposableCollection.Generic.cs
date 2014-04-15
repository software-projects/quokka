#region License

// Copyright 2004-2012 John Jeffery <john@jeffery.id.au>
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