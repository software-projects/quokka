using System;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Event subscription for publisher thread
	/// </summary>
	/// <typeparam name="TPayload"></typeparam>
	internal class PublishThreadSubscription<TPayload> : EventSubscription<TPayload>
	{
		public PublishThreadSubscription(Event<TPayload> parentEvent, Action<TPayload> action, ThreadOption threadOption,
		                                 ReferenceOption referenceOption)
			: base(parentEvent, action, threadOption, referenceOption)
		{
			if (ThreadOption != ThreadOption.PublisherThread)
			{
				throw new InvalidOperationException("Incorrect thread option");
			}
		}

		protected override void InvokeAction(Action<TPayload> action, TPayload payload)
		{
			action(payload);
		}
	}
}