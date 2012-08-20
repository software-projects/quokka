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
