using System;
using System.Collections.Generic;
using System.Threading;
using Common.Logging;
using Quokka.Util;

namespace Quokka.Stomp.Internal
{
	// Not a great name at present. Provides access to queues, sessions, etc.
	internal class ServerData : IDisposable
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly object _lockObject = new object();
		private readonly Dictionary<string, ServerSideSession> _sessions = new Dictionary<string, ServerSideSession>();
		private readonly Dictionary<string, MessageQueue> _messageQueues = new Dictionary<string, MessageQueue>();
		private readonly StompServerConfig _config = new StompServerConfig();
		private Timer _cleanupTimer;
		private bool _isDisposed;

		public StompServerConfig Config
		{
			get { return _config; }
		}

		public ServerData()
		{
			_cleanupTimer = new Timer(CleanupCallback);
		}

		public void StartCleanupTimer()
		{
			_cleanupTimer.Change(_config.CleanupPeriod, TimeSpan.FromMilliseconds(-1));
		}

		public void Dispose()
		{
			lock (_lockObject)
			{
				DisposeUtils.DisposeOf(ref _cleanupTimer);
				_isDisposed = true;
			}
		}

		public ServerSideSession FindSession(string sessionId)
		{
			lock (_lockObject)
			{
				ServerSideSession session;
				_sessions.TryGetValue(sessionId, out session);
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
				_sessions.Add(session.SessionId, session);
				return session;
			}
		}

		public void EndSession(ServerSideSession clientSession)
		{
			if (clientSession != null)
			{
				lock (_lockObject)
				{
					_sessions.Remove(clientSession.SessionId);
					clientSession.Dispose();
				}
			}
		}

		private void CleanupCallback(object state)
		{
			IEnumerable<MessageQueue> messageQueues;
			IEnumerable<ServerSideSession> sessions;
			List<MessageQueue> unusedMessageQueues = null;
			List<ServerSideSession> unusedSessions = null;

			Log.Debug("Starting cleanup");

			// We don't want to lock all of the data for the time taken to cleanup
			// everything. What we do is get a list of all message queues and a list
			// of all sessions, and rely on the locks for each during the cleanup
			// process.
			lock (_lockObject)
			{
				if (_isDisposed)
				{
					return;
				}

				messageQueues = new List<MessageQueue>(_messageQueues.Values);
				sessions = new List<ServerSideSession>(_sessions.Values);
			}

			foreach (var messageQueue in messageQueues)
			{
				messageQueue.RemoveExpired();
				if (messageQueue.IsUnused)
				{
					if (unusedMessageQueues == null)
					{
						unusedMessageQueues = new List<MessageQueue>();
					}
					unusedMessageQueues.Add(messageQueue);
				}
			}

			foreach (var session in sessions)
			{
				if (session.IsUnused)
				{
					if (unusedSessions == null)
					{
						unusedSessions = new List<ServerSideSession>();
					}
					unusedSessions.Add(session);
				}
			}

			if (unusedMessageQueues != null)
			{
				lock (_lockObject)
				{
					foreach (var messageQueue in unusedMessageQueues)
					{
						if (messageQueue.IsUnused)
						{
							_messageQueues.Remove(messageQueue.Name);
							messageQueue.Dispose();
						}
					}
				}
			}

			if (unusedSessions != null)
			{
				lock (_lockObject)
				{
					foreach (var session in unusedSessions)
					{
						if (session.IsUnused)
						{
							_sessions.Remove(session.SessionId);
							session.Dispose();
						}
					}
				}
			}

			StartCleanupTimer();
		}
	}
}