using System;
using System.Threading;
using System.Windows.Forms;

namespace Quokka.WinForms
{
	/// <summary>
	///     Simple mechanism for displaying the wait cursor
	/// </summary>
	/// <example>
	/// <code>
	/// public void LongOperation() {
	///     // about to start a long operation
	///     using (WaitCursor.Show()) {
	///         // do long operation here
	///         .
	///         .
	///         .
	///     }
	/// }
	/// </code>
	/// </example>
	/// <remarks>
	///     Multiple calls to <c>WaitCursor.Show</c> can be nested.
	/// </remarks>
	internal static class WaitCursor
	{
		private static readonly IDisposable restoreCursor = new RestoreCursor();
		private static int referenceCount;

		public static IDisposable Show()
		{
			if (Interlocked.Increment(ref referenceCount) == 1)
			{
				Cursor.Current = Cursors.WaitCursor;
			}
			return restoreCursor;
		}

		private class RestoreCursor : IDisposable
		{
			public void Dispose()
			{
				if (Interlocked.Decrement(ref referenceCount) == 0)
				{
					Cursor.Current = Cursors.Default;
				}
			}
		}
	}
}