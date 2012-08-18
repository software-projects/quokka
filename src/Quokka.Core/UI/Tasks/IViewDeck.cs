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
	public interface IViewDeck
	{
		/// <summary>
		/// 	This event is raised whenever one of the views in the deck is closed.
		/// </summary>
		event EventHandler<ViewClosedEventArgs> ViewClosed;

		///<summary>
		///	Sets the <see cref = "IViewDeck" /> into a mode where the currently displayed
		///	view can be changed.
		///</summary>
		///<returns>
		///	Returns an <see cref = "IViewTransition" /> object, which can be used to change the
		///	contents of the <see cref = "IViewDeck" />. It is essential that the caller calls
		///	<see cref = "IDisposable.Dispose" /> on this object after finishing the transition.
		///</returns>
		///<remarks>
		///	<para>
		///		It is essential that the caller calls <see cref = "IDisposable.Dispose" /> on this 
		///		object after finishing the transition.
		///	</para>
		///	<para>The best way to use this in C# is to make use of the <c>using</c> keyword.</para>
		///	<code>
		///		using (var transition = viewDeck.BeginTransition()) {
		///		transition.AddView(newView);
		///		transition.RemoveView(oldView);
		///		transition.ShowView(newView);
		///		}
		///	</code>
		///</remarks>
		IViewTransition BeginTransition(IUITask task);

		/// <summary>
		/// 	Creates a <see cref = "IModalWindow" /> object, which can be used to host
		/// 	views.
		/// </summary>
		/// <returns>
		/// 	An object that implements the <see cref = "IModalWindow" /> interface.
		/// </returns>
		/// <remarks>
		/// 	The modal window returned is not yet visible. When its <see cref = "IModalWindow.ShowModal" /> method
		/// 	is called, it will be displayed modally, and the window that the view deck is a child of will be
		/// 	disabled for the duration that the modal window is displayed.
		/// </remarks>
		IModalWindow CreateModalWindow();
	}
}