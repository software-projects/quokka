using System;
using System.Collections.Generic;
using Quokka.Diagnostics;
using Quokka.Events.Internal;

namespace Quokka.Events
{
	/// <summary>
	/// An event that supports a specify type of payload
	/// </summary>
	public class Event : EventBase
	{
		private readonly List<EventSubscription> _eventSubscriptions = new List<EventSubscription>();

		/// <summary>
		/// Subscribe to the event.
		/// </summary>
		/// <param name="action">Action to take when the event is published.</param>
		/// <returns>Returns an <see cref="IEventSubscription"/> object that represents the subscription.</returns>
		public IEventSubscription Subscribe(Action action)
		{
			return Subscribe(action, ThreadOption.PublisherThread);
		}

		/// <summary>
		/// Subscribe to the event.
		/// </summary>
		/// <param name="action">Action to take when the event is published.</param>
		/// <param name="threadOption">Specifies which thread the action will be performed on.</param>
		/// <returns>Returns an <see cref="IEventSubscription"/> object that represents the subscription.</returns>
		public IEventSubscription Subscribe(Action action, ThreadOption threadOption)
		{
			return Subscribe(action, threadOption, ReferenceOption.WeakReference);
		}

		/// <summary>
		/// Subscribe to the event.
		/// </summary>
		/// <param name="action">Action to take when the event is published.</param>
		/// <param name="threadOption">Specifies which thread the action will be performed on.</param>
		/// <param name="referenceOption">Specifies whether the event subscription will hold a strong or 
		/// weak reference on the action delegate.</param>
		/// <returns>Returns an <see cref="IEventSubscription"/> object that represents the subscription.</returns>
		public IEventSubscription Subscribe(Action action, ThreadOption threadOption,
											ReferenceOption referenceOption)
		{
			Verify.ArgumentNotNull(action, "action");
			EventSubscription eventSubscription = CreateEventSubscription(action, threadOption, referenceOption);
			lock (_eventSubscriptions)
			{
				_eventSubscriptions.Add(eventSubscription);
			}
			return eventSubscription;
		}

		/// <summary>
		/// Publish the event for all subscribers. Honour the thread option requested by each subscriber.
		/// </summary>
		public void Publish()
		{
			// lock the collection and copy to an array to avoid thread contention
			EventSubscription[] array;
			lock (_eventSubscriptions)
			{
				array = _eventSubscriptions.ToArray();
			}

			// List of event subscriptions that will be removed after the publish has completed.
			// Because the chances of there being one of these subscriptions is low, we do not
			// actually create the list until there is one item that needs to be removed.
			List<EventSubscription> removeItems = null;

			foreach (var eventSubscription in array)
			{
				// TODO: should we handle exceptions thrown during the publish here.
				if (!eventSubscription.Publish())
				{
					// Tried to publish but the event subscription is no longer valid for
					// some reason. Just remember that it has to be removed.
					if (removeItems == null)
					{
						removeItems = new List<EventSubscription>();
					}
					removeItems.Add(eventSubscription);
				}
			}

			// get rid of any obsolete event subscriptions
			if (removeItems != null)
			{
				lock (_eventSubscriptions)
				{
					foreach (var eventSubscription in removeItems)
					{
						_eventSubscriptions.Remove(eventSubscription);
					}
				}
			}
		}

		/// <summary>
		/// Create the right type of event subscription for the thread option
		/// </summary>
		private EventSubscription CreateEventSubscription(Action action, ThreadOption threadOption,
																	ReferenceOption referenceOption)
		{
			switch (threadOption)
			{
				case ThreadOption.PublisherThread:
					return new PublishThreadSubscription(this, action, threadOption, referenceOption);
				case ThreadOption.UIThread:
				case ThreadOption.UIThreadPost:
					return new UIThreadSubscription(this, action, threadOption, referenceOption);
				case ThreadOption.BackgroundThread:
					return new BackgroundThreadSubscription(this, action, threadOption, referenceOption);
			}

			// this should not happen.
			throw new ArgumentException("Unknown thread option: " + threadOption);
		}
	}
}