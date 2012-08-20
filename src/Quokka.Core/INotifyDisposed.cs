using System;

namespace Quokka
{
	/// <summary>
	/// Interface implemented by a class that notifies when it has been disposed.
	/// </summary>
	public interface INotifyDisposed : IDisposable
	{
		/// <summary>
		/// Occurs when the component is disposed by a call to the <see cref="IDisposable.Dispose"/> method
		/// </summary>
		event EventHandler Disposed;
	}
}
