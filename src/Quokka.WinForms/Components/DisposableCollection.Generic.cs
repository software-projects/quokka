using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Common.Logging;

namespace Quokka.WinForms.Components
{
	/// <summary>
	/// A collection of <see cref="IDisposable"/> objects.
	/// </summary>
	/// <typeparam name="T">Disposable object type</typeparam>
	/// <remarks>
	/// This collection implements the <see cref="IComponent"/> interface
	/// for the sole purpose that in Windows Forms, it can be convenient
	/// to add this to the components collection.
	/// <example>
	/// <code>
	/// _disposables = new DisposableCollection();
	/// components.Add(_disposables);
	/// </code>
	/// </example>
	/// </remarks>
	public class DisposableCollection<T> : Collection<T>, IComponent
		where T : IDisposable
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		public event EventHandler Disposed;

		public DisposableCollection()
		{
		}

		public DisposableCollection(IContainer container)
		{
			container.Add(this);
		}

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
						// disposed. This is correct. An object should only throw this if another
						// method is called after disposed. Disposed can be called as many times as
						// you like according to the CLR standard.
						Log.WarnFormat("Object of type {0} threw ObjectDisposedException during Dispose",
									   item.GetType().FullName);
					}
				}
			}
		}

		public ISite Site { get; set; }
	}
}