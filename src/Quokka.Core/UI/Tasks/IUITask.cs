using System;
using Quokka.ServiceLocation;
using Quokka.Uip;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// 	Represents a User Interface Task.
	/// </summary>
	/// <remarks>
	/// 	This interface contains common properties between the obsolete <see cref = "UipTask" /> and
	/// 	the newer <see cref = "UITask" />. It helps with providing some backwards compatibility.
	/// </remarks>
	public interface IUITask
	{
		event EventHandler TaskComplete;
		// TODO: could include TaskStarted event here for symmetry, but it is not needed at the moment

		bool IsRunning { get; }
		IServiceContainer ServiceContainer { get; }

		void Start(IViewDeck viewDeck);
		void EndTask();
	}
}