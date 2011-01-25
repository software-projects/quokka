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
	/// <summary>
	/// This interface describes a collection of views, where only one view is visible at at time.
	/// Because only one view is visible at a time, the concept is a 'deck' of views, where only the
	/// view at the top of the deck is visible.
	/// </summary>
	public interface IViewDeck
	{
		/// <summary>
		/// This event is raised whenever one of the views in the deck is closed.
		/// </summary>
		event EventHandler<ViewClosedEventArgs> ViewClosed;

		/// <summary>
		/// Called whenever a new task is going to make use of the view deck.
		/// </summary>
		/// <param name="task">The task that will use the view deck.</param>
		void BeginTask(object task);

		/// <summary>
		/// Called whenever a task is no longer going to use the view deck.
		/// </summary>
		/// <param name="task">The task that is no longer going to use the view deck.</param>
		void EndTask(object task);

		/// <summary>
		/// Instructs the view deck to cease painting itself.
		/// </summary>
		/// <remarks>
		/// This is intended as a hint to the view deck to remove flicker when transitioning
		/// from one view to another.
		/// </remarks>
		void BeginTransition();

		/// <summary>
		/// Instructs the view to start repainting itself again.
		/// </summary>
		/// <remarks>
		/// This method will be called shortly after a call to <see cref="BeginTransition"/>.
		/// </remarks>
		void EndTransition();

		/// <summary>
		/// Adds a view to the view deck, but does not necessarily cause it to be displayed.
		/// </summary>
		/// <param name="view">The view to add to the view deck.</param>
		void AddView(object view);

		/// <summary>
		/// Removes the view from the view deck.
		/// </summary>
		/// <param name="view">The view to remove from the view deck.</param>
		void RemoveView(object view);

		/// <summary>
		/// Causes the view to be visible. Only one view in the view deck is visible at a time.
		/// </summary>
		/// <param name="view">The view to be visible.</param>
		void ShowView(object view);

		/// <summary>
		/// Causes the view to be invisible.
		/// </summary>
		/// <param name="view">The view that should become invisible.</param>
		void HideView(object view);

		/// <summary>
		/// TODO
		/// </summary>
		/// <param name="view"></param>
		void ShowModalView(object view);
	}
}