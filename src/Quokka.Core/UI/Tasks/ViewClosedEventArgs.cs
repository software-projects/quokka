using System;
using Quokka.Diagnostics;

namespace Quokka.UI.Tasks
{
	public class ViewClosedEventArgs : EventArgs
	{
		private readonly object _view;

		public ViewClosedEventArgs(object view)
		{
			_view = Verify.ArgumentNotNull(view, "view");
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