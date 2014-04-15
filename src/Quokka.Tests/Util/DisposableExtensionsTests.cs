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
using System.ComponentModel;
using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Quokka.Util
{
	[TestFixture]
	public class DisposableExtensionsTests
	{
		[Test]
		public void DisposeWith_Component()
		{
			var disposable = new MyDisposable();
			var component = new MyComponent();
			disposable.DisposeWith(component);

			Assert.IsFalse(disposable.IsDisposed);
			component.Dispose();
			Assert.IsTrue(disposable.IsDisposed);
		}

		[Test]
		public void DisposeWith_NotifyDisposed()
		{
			var disposable = new MyDisposable();
			var component = new MyNotifyDisposed();
			disposable.DisposeWith(component);

			Assert.IsFalse(disposable.IsDisposed);
			component.Dispose();
			Assert.IsTrue(disposable.IsDisposed);
		}


		public class MyDisposable : IDisposable
		{
			public bool IsDisposed;

			public void Dispose()
			{
				IsDisposed = true;
			}
		}

		private class MyComponent : Component
		{
			
		}

		private class MyNotifyDisposed : IDisposable, INotifyDisposed
		{
			public void Dispose()
			{
				Disposed(this, EventArgs.Empty);
			}

			public event EventHandler Disposed = delegate {};
		}
	}
}
