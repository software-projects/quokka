using System;
using Quokka.Diagnostics;

namespace Quokka.Uip
{
	/// <summary>
	/// Event arguments used for <see cref="IUipViewManager.ViewClosed"/> event
	/// </summary>
	public class UipViewEventArgs : EventArgs
	{
		private readonly object _view;

		public UipViewEventArgs(object view)
		{
			Verify.ArgumentNotNull(view, "view", out _view);
		}

		/// <summary>
		/// The view that was closed. Note that this view might have already been disposed.
		/// </summary>
		public object View
		{
			get { return _view; }
		}
	}
}