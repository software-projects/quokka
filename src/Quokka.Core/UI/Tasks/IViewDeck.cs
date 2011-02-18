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
	/// 	This interface describes a collection of views, where only one view is visible at at time.
	/// 	Because only one view is visible at a time, the concept is a 'deck' of views, where only the
	/// 	view at the top of the deck is visible.
	/// </summary>
	public interface IViewDeck_Old
	{
		/// <summary>
		/// 	This event is raised whenever one of the views in the deck is closed.
		/// </summary>
		event EventHandler<ViewClosedEventArgs> ViewClosed;

		/// <summary>
		/// 	Called whenever a new task is going to make use of the view deck.
		/// </summary>
		/// <param name = "task">The task that will use the view deck.</param>
		void BeginTask(object task);

		/// <summary>
		/// 	Called whenever a task is no longer going to use the view deck.
		/// </summary>
		/// <param name = "task">The task that is no longer going to use the view deck.</param>
		void EndTask(object task);

		/// <summary>
		/// 	Instructs the view deck to cease painting itself.
		/// </summary>
		/// <remarks>
		/// 	This is intended as a hint to the view deck to remove flicker when transitioning
		/// 	from one view to another.
		/// </remarks>
		void BeginTransition();

		/// <summary>
		/// 	Instructs the view to start repainting itself again.
		/// </summary>
		/// <remarks>
		/// 	This method will be called shortly after a call to <see cref = "BeginTransition" />.
		/// </remarks>
		void EndTransition();

		/// <summary>
		/// 	Adds a view to the view deck, but does not necessarily cause it to be displayed.
		/// </summary>
		/// <param name = "view">The view to add to the view deck.</param>
		void AddView(object view);

		/// <summary>
		/// 	Removes the view from the view deck.
		/// </summary>
		/// <param name = "view">The view to remove from the view deck.</param>
		void RemoveView(object view);

		/// <summary>
		/// 	Causes the view to be visible. Only one view in the view deck is visible at a time.
		/// </summary>
		/// <param name = "view">The view to be visible.</param>
		void ShowView(object view);

		/// <summary>
		/// 	Causes the view to be invisible.
		/// </summary>
		/// <param name = "view">The view that should become invisible.</param>
		void HideView(object view);

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

	/// <summary>
	/// 	Objects that implement this interface are able to display views in a modal window.
	/// </summary>
	public interface IModalWindow : IDisposable
	{
		/// <summary>
		/// 	Occurs when the window is closed
		/// </summary>
		event EventHandler Closed;

		/// <summary>
		/// 	The view deck, in which one view at a time is displayed.
		/// </summary>
		IViewDeck ViewDeck { get; }

		/// <summary>
		/// 	Displays the window as a modal.
		/// </summary>
		void ShowModal();
	}

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
		/// Returns an <see cref="IViewTransition"/> object, which can be used to change the
		/// contents of the <see cref="IViewDeck"/>. It is essential that the caller calls
		/// <see cref="IDisposable.Dispose"/> on this object after finishing the transition.
		/// </returns>
		/// <remarks>
		/// <para>
		/// It is essential that the caller calls <see cref="IDisposable.Dispose"/> on this 
		/// object after finishing the transition.
		/// </para>
		/// <para>The best way to use this in C# is to make use of the <c>using</c> keyword.</para>
		/// <code>
		/// using (var transition = viewDeck.BeginTransition()) {
		///     transition.AddView(newView);
		///     transition.RemoveView(oldView);
		///     transition.ShowView(newView);
		/// }
		///  </code>
		/// </remarks>
		IViewTransition BeginTransition();

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

	public interface IViewTransition : IDisposable
	{
		/// <summary>
		/// 	Called whenever a new task is going to make use of the view deck.
		/// </summary>
		/// <param name = "task">The task that will use the view deck.</param>
		void BeginTask(object task);

		/// <summary>
		/// 	Called whenever a task is no longer going to use the view deck.
		/// </summary>
		/// <param name = "task">The task that is no longer going to use the view deck.</param>
		void EndTask(object task);

		/// <summary>
		/// 	Adds a view to the view deck, but does not necessarily cause it to be displayed.
		/// </summary>
		/// <param name = "view">The view to add to the view deck.</param>
		void AddView(object view);

		/// <summary>
		/// 	Removes the view from the view deck.
		/// </summary>
		/// <param name = "view">The view to remove from the view deck.</param>
		void RemoveView(object view);

		/// <summary>
		/// 	Causes the view to be visible. Only one view in the view deck is visible at a time.
		/// </summary>
		/// <param name = "view">The view to be visible.</param>
		void ShowView(object view);

		/// <summary>
		/// 	Causes the view to be invisible.
		/// </summary>
		/// <param name = "view">The view that should become invisible.</param>
		void HideView(object view);
	}
}