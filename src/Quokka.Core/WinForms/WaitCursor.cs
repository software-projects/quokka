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
using System.Threading;
using System.Windows.Forms;
using Quokka.Events;

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
	public static class WaitCursor
	{
		private static readonly IDisposable restoreCursor = new RestoreCursor();
		private static int _referenceCount;

		public static readonly Event WaitCursorNowHidden = new Event();
		public static readonly Event WaitCursorNowShowing = new Event();

		public static IDisposable Show()
		{
			if (Interlocked.Increment(ref _referenceCount) == 1)
			{
				Cursor.Current = Cursors.WaitCursor;
				WaitCursorNowShowing.Publish();
			}
			return restoreCursor;
		}

		private class RestoreCursor : IDisposable
		{
			public void Dispose()
			{
				if (Interlocked.Decrement(ref _referenceCount) == 0)
				{
					Cursor.Current = Cursors.Default;
					WaitCursorNowHidden.Publish();
				}
			}
		}
	}
}