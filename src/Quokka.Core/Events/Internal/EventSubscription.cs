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
	public abstract class EventSubscription : EventSubscriptionBase
	{
		protected EventSubscription(EventBase parentEvent,
									Action action,
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
		public Action Action
		{
			get { return (Action)DelegateReference.Delegate; }
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
		public bool Publish()
		{
			Action action;
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

			InvokeAction(action);
			return true;
		}

		protected abstract void InvokeAction(Action action);
	}
}