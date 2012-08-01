using System;
using System.Threading;

namespace Quokka.UI
{
	/// <summary>
	/// Static class for interacting with the main UI thread.
	/// </summary>
	public class UIThread
	{
		/// <summary>
		/// The Synchronization context for the UI thread.
		/// </summary>
		/// <remarks>
		/// This is set very early on in the Bootstrap sequence for a UI application.
		/// Although the property is public writable, the intention is that it should
		/// be set once and then left. Unless you are writing your own bootstrapper, do
		/// not write to this property.
		/// </remarks>
		public static SynchronizationContext SynchronizationContext { get; set; }

		/// <summary>
		/// Perform an action on the UI thread synchronously.
		/// </summary>
		/// <param name="action">Action to perform.</param>
		public static void Send(Action action)
		{
			CheckSynchronizationContext();
			SynchronizationContext.Send(state => action(), null);
		}

		/// <summary>
		/// Perform an action on the UI thread asynchronously.
		/// </summary>
		/// <param name="action">Action to perform.</param>
		public static void Post(Action action)
		{
			CheckSynchronizationContext();
			SynchronizationContext.Post(state => action(), null);
		}

		private static void CheckSynchronizationContext()
		{
			if (SynchronizationContext == null)
			{
				const string msg = "UIThread.SynchronizationContext is null, so cannot send/post to the UI thread.";
				throw new InvalidOperationException(msg);
			}
		}
	}
}