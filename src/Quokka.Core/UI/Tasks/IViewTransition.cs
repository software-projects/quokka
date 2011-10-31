using System;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Created by an <see cref = "IViewDeck" /> in order to effect a transition from one view to another.
	/// </summary>
	public interface IViewTransition : IDisposable
	{
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