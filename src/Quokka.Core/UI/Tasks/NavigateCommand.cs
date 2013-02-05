#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

using System;

namespace Quokka.UI.Tasks
{
	///<summary>
	///	Used by <see cref = "Presenter{T}" /> classes to navigate between nodes (ie <see cref = "UINode" /> objects)
	///	in the <see cref = "UITask" />.
	///</summary>
	///<example>
	///	<code>
	///		class MyPresenter : Presenter&lt;IMyView&gt; {
	///		public readonly NavigateCommand Next = new NavigateCommand();
	///		public readonly NavigateCommand Cancel = new NavigateCommand();
	///
	///		//
	///		// ... skip code here
	///		//
	/// 
	///		public void OnNext(object sender, EventArgs e) {
	///		// ... do some processing here
	/// 
	///		// Navigate to the next node, wherever that will be
	///		Next.Navigate();
	///		}
	/// 
	///		public void OnCance(object sender, EventArgs e) {
	///		// Navigate to the cancel node
	///		Cancel.Navigate();
	///		}
	///		}
	///	</code>
	///</example>
	internal sealed class NavigateCommand : INavigateCommand, IDisposable
	{
		public event EventHandler Navigating;
		public event EventHandler NavigationNotDefined;

		internal UINode FromNode { get; set; }
		internal UINode ToNode { get; set; }

		public bool CanNavigate
		{
			get { return Navigating != null; }
		}

		public void Navigate()
		{
			UIThread.Send(NavigateOnUIThread);
		}

		private void NavigateOnUIThread()
		{
			// do not navigate if we have been disposed
			if (!_isDisposed)
			{
				if (Navigating == null)
				{
					if (NavigationNotDefined != null)
					{
						NavigationNotDefined(this, EventArgs.Empty);
					}
					else
					{
						// This should not get called, because the UINode should always subscribe
						// to the NavigationNotDefined event.
						throw new InvalidOperationException("No transition is defined for this navigate command.");
					}
				}
				else
				{
					Navigating(this, EventArgs.Empty);
				}
			}			
		}

		private bool _isDisposed;

		public void Dispose()
		{
			_isDisposed = true;
		}
	}
}