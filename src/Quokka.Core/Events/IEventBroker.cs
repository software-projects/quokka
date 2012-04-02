using System;
using System.Threading;
using Quokka.ServiceLocation;

namespace Quokka.Events
{
	/// <summary>
	/// Provides a broker service for finding instances of events.
	/// </summary>
	/// <remarks>
	/// This interface is available via the service locator (<see cref="ServiceLocator"/>,
	/// or via dependency injection.
	/// </remarks>
	public interface IEventBroker
	{
		/// <summary>
		/// Find an instance of an event.
		/// </summary>
		/// <typeparam name="TEvent">Type of the event to find.</typeparam>
		/// <returns>
		/// The event instance. If the event instance did not exist previously, it
		/// is created.
		/// </returns>
		TEvent GetEvent<TEvent>() where TEvent : EventBase, new();

		/// <summary>
		/// Find an instance of an event.
		/// </summary>
		/// <param name="eventType">
		/// The type of event. This should be a subtype of <see cref="EventBase"/>.
		/// </param>
		/// <returns>
		/// The event instance. If the event instance did not exist previously, it
		/// is created.
		/// </returns>
		EventBase GetEvent(Type eventType);

		/// <summary>
		/// The synchronization context for the UI thread.
		/// </summary>
		SynchronizationContext UIThreadContext { get; set; }
	}
}