using System;

namespace Quokka.Events
{
	public interface IEventSubscription<TPayload> : IEventSubscription
	{
		IEventSubscription<TPayload> SetFilter(Func<TPayload, bool> filter);
	}
}