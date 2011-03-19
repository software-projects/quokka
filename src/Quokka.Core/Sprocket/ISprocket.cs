using System;
using System.Threading;

namespace Quokka.Sprocket
{
	public interface ISprocket
	{
		event EventHandler ConnectedChanged;
		bool Connected { get; }
		Uri ServerUrl { get; }
		SynchronizationContext SynchronizationContext { get; set; }

		void Open(Uri serverUrl);
		void Close();

		void Publish(object message);

		bool CanReply { get; }
		void Reply(object message);

		ISubscriber<T> CreateSubscriber<T>();
		IPublisher<T> CreatePublisher<T>();
		IChannel CreateChannel();
	}
}