using System;
using System.Threading;
using Quokka.Diagnostics;

namespace Quokka.Sprocket
{
	public partial class SprocketClient
	{
		// TODO: This needs to be much smarter, so that it knows when to publish.
		// Currently it just thinks it has to publish all the time.
		private class Publisher<T> : IPublisher<T>
		{
			private readonly SprocketClient _client;

			public event EventHandler SubscribedChanged;

			public Publisher(SprocketClient client)
			{
				_client = Verify.ArgumentNotNull(client, "client");
				SynchronizationContext = _client.SynchronizationContext;
			}

			public void Dispose()
			{
				
			}

			public ISprocket Sprocket
			{
				get { return _client; }
			}

			public SynchronizationContext SynchronizationContext { get; set; }

			public bool Subscribed
			{
				get { return true; }
			}

			public void Publish(T obj)
			{
				_client.Publish(obj);
			}
		}
	}
}
