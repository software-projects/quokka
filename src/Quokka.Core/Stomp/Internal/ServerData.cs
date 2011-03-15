using System;
using System.Collections.Generic;
using System.Threading;
using Common.Logging;
using Quokka.Stomp.Server.Messages;
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
		private MessageQueue _serverStatusMessageQueue;
		private MessageQueue _messageLogMessageQueue;
		private readonly StompServerConfig _config = new StompServerConfig();
		private Timer _cleanupTimer;
		private Timer _serverStatusTimer;
		private bool _isDisposed;

		public StompServerConfig Config
		{
			get { return _config; }
		}

		public ServerData()
		{
			_cleanupTimer = new Timer(CleanupCallback);
			_serverStatusTimer = new Timer(ServerStatusCallback);
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

					if (mq.Name == typeof(ServerStatusMessage).FullName)
					{
						// special message queue name -- receives status messages
						_serverStatusMessageQueue = mq;

						// start the timer to fire soon
						_serverStatusTimer.Change(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(-1));
					}
					else if (mq.Name == typeof(MessageLogMessage).FullName)
					{
						_messageLogMessageQueue = mq;
					}
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

		public void EndSession(ServerSideSession session)
		{
			if (session != null)
			{
				lock (_lockObject)
				{
					_sessions.Remove(session.SessionId);
					session.Dispose();
				}
			}
		}

		public void LogSendMessage(StompFrame frame, string destination)
		{
			if (_messageLogMessageQueue != null)
			{
				lock(_lockObject)
				{
					if (_messageLogMessageQueue != null)
					{
						var msg = new MessageLogMessage
						          	{
						          		SentAt = DateTime.Now,
						          		ContentLength = frame.GetInt32(StompHeader.ContentLength, 0),
						          		Destination = destination,
						          	};
						var msgFrame = new StompFrame(StompCommand.Message)
						               	{
						               		Headers =
						               			{
						               				{StompHeader.Destination, _messageLogMessageQueue.Name}
						               			}
						               	};
						msgFrame.Serialize(msg);
						_messageLogMessageQueue.PublishFrame(msgFrame);
					}
				}
			}
		}

		private void ServerStatusCallback(object state)
		{
			lock (_lockObject)
			{
				if (_serverStatusMessageQueue == null)
				{
					return;
				}

				var message = new ServerStatusMessage
				              	{
				              		MessageQueues = new List<MessageQueueStatus>(),
				              		Sessions = new List<SessionStatus>(),
				              	};

				foreach (var session in _sessions.Values)
				{
					var status = session.CreateSessionStatus();
					message.Sessions.Add(status);
				}
				foreach (var messageQueue in _messageQueues.Values)
				{
					var status = messageQueue.CreateStatus();
					message.MessageQueues.Add(status);
				}

				var frame = new StompFrame(StompCommand.Message);
				frame.Serialize(message);
				frame.SetExpires(_config.ServerStatusPeriod);
				_serverStatusMessageQueue.PublishFrame(frame);

				_serverStatusTimer.Change(_config.ServerStatusPeriod, TimeSpan.FromMilliseconds(-1));
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

			if (_serverStatusMessageQueue != null && _serverStatusMessageQueue.IsDisposed)
			{
				_serverStatusMessageQueue = null;
			}

			if (_messageLogMessageQueue != null && _messageLogMessageQueue.IsDisposed)
			{
				_messageLogMessageQueue = null;
			}

			StartCleanupTimer();
		}
	}
}