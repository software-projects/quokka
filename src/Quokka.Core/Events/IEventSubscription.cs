using System;

namespace Quokka.Events
{
	public interface IEventSubscription : IDisposable
	{
		bool IsSubscribed { get; }
		Type EventType { get; }
		ThreadOption ThreadOption { get; }
		ReferenceOption ReferenceOption { get; }
		void Unsubscribe();
	}
}