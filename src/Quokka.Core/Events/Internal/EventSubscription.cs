using System;
using Quokka.Diagnostics;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Implementation class for <see cref="IEventSubscription"/>
	/// </summary>
	/// <typeparam name="TPayload">Event payload type.</typeparam>
	public abstract class EventSubscription<TPayload> : IEventSubscription
	{
		private readonly Event<TPayload> _event;
		private DelegateReference _delegateReference;
		public event EventHandler Unsubscribed;

		public EventSubscription(Event<TPayload> parentEvent, Action<TPayload> action, ThreadOption threadOption,
		                         ReferenceOption referenceOption)
		{
			Verify.ArgumentNotNull(parentEvent, "parentEvent", out _event);
			Verify.ArgumentNotNull(action, "action");
			_delegateReference = new DelegateReference(action, referenceOption);
			ThreadOption = threadOption;
		}

		public bool IsSubscribed { get; private set; }
		public ThreadOption ThreadOption { get; private set; }

		public Event<TPayload> Event
		{
			get { return _event; }
		}

		public ReferenceOption ReferenceOption
		{
			get { return _delegateReference.ReferenceOption; }
		}

		public Type EventType
		{
			get { return _event.GetType(); }
		}

		public Type PayloadType
		{
			get { return typeof (TPayload); }
		}

		/// <summary>
		/// Action to take when the event is published, or 
		/// </summary>
		public Action<TPayload> Action
		{
			get { return (Action<TPayload>) _delegateReference.Delegate; }
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
			if (!IsSubscribed)
			{
				// This event subscription has been unsubscribed, so do nothing.
				return false;
			}
			Action<TPayload> action = Action;
			if (action == null)
			{
				// Weak reference has been garbage collected
				return false;
			}

			InvokeAction(action, payload);
			return true;
		}

		/// <summary>
		/// Unsubscribe from the event
		/// </summary>
		public void Unsubscribe()
		{
			_delegateReference = null;
			IsSubscribed = false;

			if (Unsubscribed != null)
			{
				Unsubscribed(this, EventArgs.Empty);
			}
		}

		protected abstract void InvokeAction(Action<TPayload> action, TPayload payload);
	}
}