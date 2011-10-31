using System;
using Quokka.Diagnostics;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Common functionality between the generic <see cref="EventSubscription{TPayload}"/> and non-generic <see cref="EventSubscription"/> class.
	/// </summary>
	public class EventSubscriptionBase : IEventSubscription
	{
		private readonly EventBase _event;
		public event EventHandler Unsubscribed;
		private readonly object _lockObject = new object();

		void IDisposable.Dispose()
		{
			Unsubscribe();
		}

		protected EventSubscriptionBase(EventBase parentEvent, ThreadOption threadOption, ReferenceOption referenceOption)
		{
			Verify.ArgumentNotNull(parentEvent, "parentEvent", out _event);
			IsSubscribed = true;
			ThreadOption = threadOption;
		}

		public bool IsSubscribed { get; private set; }
		public ThreadOption ThreadOption { get; private set; }

		public EventBase Event
		{
			get { return _event; }
		}

		public ReferenceOption ReferenceOption
		{
			get { return DelegateReference.ReferenceOption; }
		}

		public Type EventType
		{
			get { return _event.GetType(); }
		}

		/// <summary>
		/// Unsubscribe from the event
		/// </summary>
		public void Unsubscribe()
		{
			lock (_lockObject)
			{
				IsSubscribed = false;
				DelegateReference = null;
			}

			if (Unsubscribed != null)
			{
				Unsubscribed(this, EventArgs.Empty);
			}
		}

		protected DelegateReference DelegateReference { get; set; }

		protected object LockObject
		{
			get { return _lockObject; }
		}
	}
}