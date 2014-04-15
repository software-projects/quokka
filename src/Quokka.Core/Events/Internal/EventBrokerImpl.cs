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
	public class EventBrokerImpl : IEventBroker
	{
		private readonly Dictionary<Type, EventBase> _events = new Dictionary<Type, EventBase>();

		public EventBrokerImpl()
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