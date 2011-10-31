using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Quokka.Collections;
using Quokka.Stomp.Server.Messages;

namespace Quokka.Sprocket
{
	class Example
	{
		public ISprocket Sprocket { get; set; }
		public readonly DisposableCollection Disposables = new DisposableCollection();
		public SynchronizationContext SynchronizationContext { get; set; }

		public void Initialize()
		{
			var channel = Sprocket.CreateChannel()
				.WithSynchronizationContext(SynchronizationContext)
				.HandleResponse<ServerStatusMessage>(HandleServerStatusMessage)
				.HandleResponse<MessageLogMessage>(HandleMessageLogMessage)
				.AddTo(Disposables);

			var publisher = Sprocket.CreatePublisher<ServerStatusMessage>()
				.WithSynchronizationContext(SynchronizationContext)
				.AddTo(Disposables);

			channel.Send(new ServerStatusMessage());

			publisher.Publish(new ServerStatusMessage());


		}

		private void HandleServerStatusMessage(ServerStatusMessage msg)
		{
		}

		private void HandleMessageLogMessage(MessageLogMessage msg)
		{
			
		}
	}
}
