using System;

namespace Quokka.Events
{
	public interface IEventSubscription
	{
		bool IsSubscribed { get; }
		Type EventType { get; }
		ThreadOption ThreadOption { get; }
		ReferenceOption ReferenceOption { get; }

		void Unsubscribe();
	}
}