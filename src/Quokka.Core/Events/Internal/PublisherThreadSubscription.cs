using System;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Event subscription for publisher thread
	/// </summary>
	internal class PublishThreadSubscription : EventSubscription
	{
		public PublishThreadSubscription(EventBase parentEvent,
		                                 Action action,
		                                 ThreadOption threadOption,
		                                 ReferenceOption referenceOption)
			: base(parentEvent, action, threadOption, referenceOption)
		{
			if (ThreadOption != ThreadOption.PublisherThread)
			{
				throw new InvalidOperationException("Incorrect thread option");
			}
		}

		protected override void InvokeAction(Action action)
		{
			action();
		}
	}
}