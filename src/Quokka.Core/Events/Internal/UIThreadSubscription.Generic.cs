using System;
using System.Threading;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Event subscription for the UI thread options.
	/// </summary>
	/// <typeparam name="TPayload"></typeparam>
	internal class UIThreadSubscription<TPayload> : EventSubscription<TPayload>
	{
		public UIThreadSubscription(Event<TPayload> parentEvent, Action<TPayload> action, ThreadOption threadOption,
		                            ReferenceOption referenceOption)
			: base(parentEvent, action, threadOption, referenceOption)
		{
			if (ThreadOption != ThreadOption.UIThread && ThreadOption != ThreadOption.UIThreadPost)
			{
				throw new InvalidOperationException("Incorrect thread option");
			}
		}

		protected override void InvokeAction(Action<TPayload> action, TPayload payload)
		{
			SendOrPostCallback callback = delegate { action(payload); };
			if (ThreadOption == ThreadOption.UIThread)
			{
				Event.EventBroker.UIThreadContext.Send(callback, null);
			}
			else
			{
				Event.EventBroker.UIThreadContext.Post(callback, null);
			}
		}
	}
}