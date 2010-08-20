using System;
using Quokka.Uip;

namespace Quokka.UI.Tasks
{
	public interface IViewDeck
	{
		event EventHandler<ViewClosedEventArgs> ViewClosed;

		void BeginTask(object task);
		void EndTask(object task);
		void BeginTransition();
		void EndTransition();
		void AddView(object view);
		void RemoveView(object view);
		void ShowView(object view);
		void HideView(object view);
		void ShowModalView(object view);
	}
}