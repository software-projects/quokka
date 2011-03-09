using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quokka.Stomp.Internal
{
	// Not a great name at present. Provides access to queues, sessions, etc.
	internal class ServerData
	{
		private readonly object _lockObject = new object();
		private readonly Dictionary<string, ClientSession> _clientSessions = new Dictionary<string, ClientSession>();

		public ClientSession FindSession(string sessionId)
		{
			throw new NotImplementedException();
		}

		public MessageQueue FindMessageQueue(string messageQueueName)
		{
			throw new NotImplementedException();
		}

		public ClientSession CreateSession()
		{
			throw new NotImplementedException();
		}

		public void EndSession(ClientSession clientSession)
		{
			if (clientSession != null)
			{
				lock (_lockObject)
				{
					_clientSessions.Remove(clientSession.SessionId);
					clientSession.Dispose();
				}
			}
		}
	}
}
