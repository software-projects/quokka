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
using Quokka.Diagnostics;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Implementation class for <see cref="IEventSubscription"/>
	/// </summary>
	/// <typeparam name="TPayload">Event payload type.</typeparam>
	public abstract class EventSubscription<TPayload> : EventSubscriptionBase, IEventSubscription<TPayload>
	{
		private DelegateReference _filterReference;

		protected EventSubscription(EventBase parentEvent,
		                            Action<TPayload> action,
		                            ThreadOption threadOption,
		                            ReferenceOption referenceOption)
			: base(parentEvent, threadOption, referenceOption)
		{
			Verify.ArgumentNotNull(action, "action");
			DelegateReference = new DelegateReference(action, referenceOption);
		}

		/// <summary>
		/// Action to take when the event is published, or 
		/// </summary>
		public Action<TPayload> Action
		{
			get { return (Action<TPayload>) DelegateReference.Delegate; }
		}

		public Func<TPayload, bool> Filter
		{
			get
			{
				Func<TPayload, bool> result = null;
				if (_filterReference != null)
				{
					result = (Func<TPayload, bool>) _filterReference.Delegate;
				}

				if (result == null)
				{
					// no filter or it has been garbage collected
					result = (payload) => true;
				}

				return result;
			}
		}

		public IEventSubscription<TPayload> SetFilter(Func<TPayload, bool> filter)
		{
			_filterReference = filter == null ? null : new DelegateReference(filter, ReferenceOption);
			return this;
		}

		/// <summary>
		/// Publish the payload to the event subscriber
		/// </summary>
		/// <param name="payload">Payload of event</param>
		/// <returns>
		/// Returns <c>true</c> if the event subscriber received the payload, or <c>false</c>
		/// if the event subscription is no longer current (either through unsubscription or
		/// by a weak reference being garbage collected).
		/// </returns>
		public bool Publish(TPayload payload)
		{
			Action<TPayload> action;
			lock (LockObject)
			{
				if (!IsSubscribed)
				{
					// This event subscription has been unsubscribed, so do nothing.
					return false;
				}
				action = Action;
			}

			if (action == null)
			{
				// Weak reference has been garbage collected
				return false;
			}

			if (Filter(payload))
			{
				InvokeAction(action, payload);
			}

			// Returns true even if the filter did not match.
			return true;
		}

		protected abstract void InvokeAction(Action<TPayload> action, TPayload payload);
	}
}