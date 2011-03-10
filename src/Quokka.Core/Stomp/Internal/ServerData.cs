using System.Collections.Generic;

namespace Quokka.Stomp.Internal
{
	// Not a great name at present. Provides access to queues, sessions, etc.
	internal class ServerData
	{
		private readonly object _lockObject = new object();
		private readonly Dictionary<string, ServerSideSession> _clientSessions = new Dictionary<string, ServerSideSession>();
		private readonly Dictionary<string, MessageQueue> _messageQueues = new Dictionary<string, MessageQueue>();

		public ServerSideSession FindSession(string sessionId)
		{
			lock (_lockObject)
			{
				ServerSideSession session;
				_clientSessions.TryGetValue(sessionId, out session);
				return session;
			}
		}

		public MessageQueue FindMessageQueue(string messageQueueName)
		{
			lock (_lockObject)
			{
				MessageQueue mq;
				if (!_messageQueues.TryGetValue(messageQueueName, out mq))
				{
					mq = new MessageQueue(messageQueueName);
					_messageQueues.Add(messageQueueName, mq);
				}
				return mq;
			}
		}

		public ServerSideSession CreateSession()
		{
			lock (_lockObject)
			{
				var session = new ServerSideSession(this);
				_clientSessions.Add(session.SessionId, session);
				return session;
			}
		}

		public void EndSession(ServerSideSession clientSession)
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