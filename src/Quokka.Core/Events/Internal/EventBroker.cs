using System;
using System.Collections.Generic;
using System.Threading;
using Quokka.Diagnostics;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Default event broker implementation.
	/// </summary>
	/// <remarks>
	/// This class assumes that it is created on the UI thread at program initialization.
	/// If this is not the case, (eg initialization occurs on a background thread), then
	/// the calling program should set the <see cref="UIThreadContext"/> property to the
	/// correct value as soon as possible.
	/// </remarks>
	public class EventBroker : IEventBroker
	{
		private readonly Dictionary<Type, EventBase> _events = new Dictionary<Type, EventBase>();

		public EventBroker()
		{
			// Assume that this object was created on the UI thread. If this is not the case,
			// then the calling program should set this property from the UI thread ASAP.
			UIThreadContext = SynchronizationContext.Current;
		}

		public TEvent GetEvent<TEvent>() where TEvent : EventBase, new()
		{
			return (TEvent) GetEvent(typeof (TEvent));
		}

		public EventBase GetEvent(Type eventType)
		{
			Verify.ArgumentNotNull(eventType, "eventType");
			if (eventType.IsAssignableFrom(typeof(EventBase)))
			{
				throw new ArgumentException("eventType should inherit from EventBase", "eventType");
			}

			lock (_events)
			{
				EventBase @event;
				if (!_events.TryGetValue(eventType, out @event))
				{
					@event = (EventBase) Activator.CreateInstance(eventType);
					@event.EventBroker = this;
					_events.Add(eventType, @event);
				}
				return @event;
			}
		}

		public SynchronizationContext UIThreadContext { get; set; }
	}
}