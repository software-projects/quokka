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
using System.Collections.Generic;
using Quokka.Diagnostics;
using Quokka.Events.Internal;

namespace Quokka.Events
{
	/// <summary>
	/// An event that supports a specify type of payload
	/// </summary>
	/// <typeparam name="TPayload">The type of payload associated with the event.</typeparam>
	public class Event<TPayload> : EventBase
	{
		private readonly List<EventSubscription<TPayload>> _eventSubscriptions = new List<EventSubscription<TPayload>>();

		/// <summary>
		/// Subscribe to the event.
		/// </summary>
		/// <param name="action">Action to take when the event is published.</param>
		/// <returns>Returns an <see cref="IEventSubscription"/> object that represents the subscription.</returns>
		public IEventSubscription<TPayload> Subscribe(Action<TPayload> action)
		{
			return Subscribe(action, ThreadOption.PublisherThread);
		}

		/// <summary>
		/// Subscribe to the event.
		/// </summary>
		/// <param name="action">Action to take when the event is published.</param>
		/// <param name="threadOption">Specifies which thread the action will be performed on.</param>
		/// <returns>Returns an <see cref="IEventSubscription"/> object that represents the subscription.</returns>
		public IEventSubscription<TPayload> Subscribe(Action<TPayload> action, ThreadOption threadOption)
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
		public IEventSubscription<TPayload> Subscribe(Action<TPayload> action, ThreadOption threadOption,
		                                    ReferenceOption referenceOption)
		{
			Verify.ArgumentNotNull(action, "action");
			EventSubscription<TPayload> eventSubscription = CreateEventSubscription(action, threadOption, referenceOption);
			lock (_eventSubscriptions)
			{
				_eventSubscriptions.Add(eventSubscription);
			}
			return eventSubscription;
		}

		/// <summary>
		/// Publish the event for all subscribers. Honour the thread option requested by each subscriber.
		/// </summary>
		/// <param name="payload">Event payload.</param>
		public void Publish(TPayload payload)
		{
			// lock the collection and copy to an array to avoid thread contention
			EventSubscription<TPayload>[] array;
			lock (_eventSubscriptions)
			{
				array = _eventSubscriptions.ToArray();
			}

			// List of event subscriptions that will be removed after the publish has completed.
			// Because the chances of there being one of these subscriptions is low, we do not
			// actually create the list until there is one item that needs to be removed.
			List<EventSubscription<TPayload>> removeItems = null;

			foreach (var eventSubscription in array)
			{
				// TODO: should we handle exceptions thrown during the publish here.
				if (!eventSubscription.Publish(payload))
				{
					// Tried to publish but the event subscription is no longer valid for
					// some reason. Just remember that it has to be removed.
					if (removeItems == null)
					{
						removeItems = new List<EventSubscription<TPayload>>();
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
		private EventSubscription<TPayload> CreateEventSubscription(Action<TPayload> action, ThreadOption threadOption,
		                                                            ReferenceOption referenceOption)
		{
			switch (threadOption)
			{
				case ThreadOption.PublisherThread:
					return new PublishThreadSubscription<TPayload>(this, action, threadOption, referenceOption);
				case ThreadOption.UIThread:
				case ThreadOption.UIThreadPost:
					return new UIThreadSubscription<TPayload>(this, action, threadOption, referenceOption);
				case ThreadOption.BackgroundThread:
					return new BackgroundThreadSubscription<TPayload>(this, action, threadOption, referenceOption);
			}

			// this should not happen.
			throw new ArgumentException("Unknown thread option: " + threadOption);
		}
	}
}