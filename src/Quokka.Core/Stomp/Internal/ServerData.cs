#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Threading;
using Castle.Core.Logging;
using Quokka.Diagnostics;
using Quokka.Stomp.Server.Messages;
using Quokka.Util;

namespace Quokka.Stomp.Internal
{
	// Not a great name at present. Provides access to queues, sessions, etc.
	internal class ServerData : IDisposable
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private readonly LockObject _lockObject = GlobalLock.LockObject;
		private readonly Dictionary<string, ServerSideSession> _sessions = new Dictionary<string, ServerSideSession>();
		private readonly Dictionary<string, MessageQueue> _messageQueues = new Dictionary<string, MessageQueue>();
		private volatile MessageQueue _serverStatusMessageQueue;
		private volatile MessageQueue _messageLogMessageQueue;
		private readonly StompServerConfig _config = new StompServerConfig();
		private Timer _cleanupTimer;
		private readonly Timer _serverStatusTimer;
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
			using (_lockObject.Lock())
			{
				DisposeUtils.DisposeOf(ref _cleanupTimer);
				_isDisposed = true;
			}
		}

		public ServerSideSession FindSession(string sessionId)
		{
			using (_lockObject.Lock())
			{
				ServerSideSession session;
				_sessions.TryGetValue(sessionId, out session);
				return session;
			}
		}

		public void RemoveSession(string sessionId)
		{
			using (_lockObject.Lock())
			{
				_sessions.Remove(sessionId);
			}
		}

		public MessageQueue FindMessageQueue(string messageQueueName)
		{
			using (_lockObject.Lock())
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
			using (_lockObject.Lock())
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
				using (_lockObject.Lock())
				{
					_sessions.Remove(session.SessionId);
					session.Dispose();
				}
			}
		}

		public void LogSendMessage(StompFrame frame, string destination)
		{
			MessageQueue messageLoggingQueue = null;

			if (_messageLogMessageQueue != null)
			{
				using (_lockObject.Lock())
				{
// ReSharper disable ConditionIsAlwaysTrueOrFalse
					if (_messageLogMessageQueue != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
					{
						messageLoggingQueue = _messageLogMessageQueue;
					}
				}
			}

			if (messageLoggingQueue != null)
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
				               				{StompHeader.Destination, messageLoggingQueue.Name}
				               			}
				               	};
				msgFrame.Serialize(msg);



				messageLoggingQueue.PublishFrame(msgFrame);
			}
		}

		private void ServerStatusCallback(object state)
		{
			List<ServerSideSession> sessions;
			List<MessageQueue> messageQueues;

			using (_lockObject.Lock())
			{
				if (_serverStatusMessageQueue == null)
				{
					return;
				}

				sessions = new List<ServerSideSession>(_sessions.Values);
				messageQueues = new List<MessageQueue>(_messageQueues.Values);
			}

			// need to be unlocked to perform this, otherwise we could deadlock


			var message = new ServerStatusMessage
			              	{
			              		MessageQueues = new List<MessageQueueStatus>(),
			              		Sessions = new List<SessionStatus>(),
			              	};

			foreach (var session in sessions)
			{
				var status = session.CreateSessionStatus();
				message.Sessions.Add(status);
			}
			foreach (var messageQueue in messageQueues)
			{
				var status = messageQueue.CreateStatus();
				message.MessageQueues.Add(status);
			}

			var frame = new StompFrame(StompCommand.Message);
			frame.Serialize(message);
			frame.SetExpires(_config.ServerStatusPeriod);

			MessageQueue queue;
			using (_lockObject.Lock())
			{
				queue = _serverStatusMessageQueue;
			}

// ReSharper disable ConditionIsAlwaysTrueOrFalse
			if (queue != null)
// ReSharper restore ConditionIsAlwaysTrueOrFalse
			{
				queue.PublishFrame(frame);
				_serverStatusTimer.Change(_config.ServerStatusPeriod, TimeSpan.FromMilliseconds(-1));
			}
		}

		private void CleanupCallback(object state)
		{
			IEnumerable<MessageQueue> messageQueues;
			IEnumerable<ServerSideSession> sessions;
			List<MessageQueue> unusedMessageQueues = null;
			List<ServerSideSession> unusedSessions = null;

			// We don't want to lock all of the data for the time taken to cleanup
			// everything. What we do is get a list of all message queues and a list
			// of all sessions, and rely on the locks for each during the cleanup
			// process.
			using (_lockObject.Lock())
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
					using (_lockObject.Lock())
					{
						if (messageQueue.IsUnused)
						{
							Log.Debug("Clean up message queue " + messageQueue.Name);
							_messageQueues.Remove(messageQueue.Name);
							if (unusedMessageQueues == null)
							{
								unusedMessageQueues = new List<MessageQueue>();
							}
							unusedMessageQueues.Add(messageQueue);
						}
					}
				}
			}

			foreach (var session in sessions)
			{
				if (session.Cleanup())
				{
					// This session has now been removed from the _sessions collection
					Log.Debug("Clean up session " + session.SessionId + ": " + session.ClientId);

					if (unusedSessions == null)
					{
						unusedSessions = new List<ServerSideSession>();
					}
					unusedSessions.Add(session);
				}
			}

			if (unusedMessageQueues != null)
			{
				foreach (var messageQueue in unusedMessageQueues)
				{
					messageQueue.Dispose();
				}
			}

			if (unusedSessions != null)
			{
				foreach (var session in unusedSessions)
				{
					session.Dispose();
				}
			}

			using (_lockObject.Lock())
			{

				if (_serverStatusMessageQueue != null && _serverStatusMessageQueue.IsDisposed)
				{
					_serverStatusMessageQueue = null;
				}

				if (_messageLogMessageQueue != null && _messageLogMessageQueue.IsDisposed)
				{
					_messageLogMessageQueue = null;
				}
			}

			StartCleanupTimer();
		}
	}
}