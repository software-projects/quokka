﻿#region License

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
using Castle.Core.Logging;
using Quokka.Diagnostics;
using Quokka.Stomp.Server.Messages;

namespace Quokka.Stomp.Internal
{
	/// <summary>
	/// 	Wraps a connection with a STOMP client and performs session management.
	/// </summary>
	internal class ServerSideSession : IDisposable
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private readonly LockObject _lockObject = GlobalLock.LockObject;
		private readonly ServerData _serverData;
		private ServerSideConnection _clientConnection;
		private readonly Dictionary<string, ServerSideSubscription> _subscriptions = new Dictionary<string, ServerSideSubscription>();
		private DateTime _expiresAt;

		public string SessionId { get; private set; }

		// this is only used to help with debugging
		public string ClientId { get; set; }


		public ServerSideSession(ServerData serverData)
		{
			SessionId = Guid.NewGuid().ToString();
			_serverData = Verify.ArgumentNotNull(serverData, "serverData");
		}

		public void Dispose()
		{
			using (_lockObject.Lock())
			{
				_clientConnection = null;
				foreach (var subscription in _subscriptions.Values)
				{
					subscription.Dispose();
				}
				_subscriptions.Clear();
			}
		}

		public SessionStatus CreateSessionStatus()
		{
			var status = new SessionStatus
			             	{
			             		SessionId = SessionId,
								ClientId = ClientId,
								Subscriptions = new List<SubscriptionStatus>(),
			             	};
			using (_lockObject.Lock())
			{
				status.Connected = _clientConnection != null;
				foreach (var subscription in _subscriptions.Values)
				{
					status.Subscriptions.Add(subscription.CreateStatus());
				}
			}
			return status;
		}

		public bool IsConnected
		{
			get { return _clientConnection != null; }
		}

		public bool Cleanup()
		{
			using (_lockObject.Lock())
			{
				if (_clientConnection == null)
				{
					if (DateTime.Now >= _expiresAt)
					{
						_serverData.RemoveSession(SessionId);
						return true;
					}
				}
			}
			return false;
		}

		public override string ToString()
		{
			return SessionId;
		}

		public bool AddConnection(ServerSideConnection clientConnection)
		{
			Verify.ArgumentNotNull(clientConnection, "clientConnection");
			IEnumerable<ServerSideSubscription> subscriptions = null;

			using (_lockObject.Lock())
			{
				if (_clientConnection != null)
				{
					return false;
				}
				_clientConnection = clientConnection;
				_clientConnection.ConnectionClosed += ClientConnectionClosed;

				if (_subscriptions.Count > 0)
				{
					subscriptions = new List<ServerSideSubscription>(_subscriptions.Values);
				}
			}

			if (subscriptions != null)
			{
				foreach (var subscription in subscriptions)
				{
					subscription.ReceiveMessagesFromQueue();
				}
			}

			return true;
		}

		private void ClientConnectionClosed(object sender, EventArgs e)
		{
			using (_lockObject.Lock())
			{
				_clientConnection = null;

				// If there is still no connection at this time, the session will
				// be considered unused.
				_expiresAt = DateTime.Now + _serverData.Config.UnusedSessionTimeout;
			}
		}

		/// <summary>
		/// Note that the caller handles exceptions here
		/// </summary>
		/// <param name="frame"></param>
		public void ProcessFrame(StompFrame frame)
		{
			using (_lockObject.Lock())
			{
				ProcessFrameByCommand(frame);
			}
		}

		private void ProcessFrameByCommand(StompFrame frame)
		{
			// Note that we do not process the disconnect command here, as it is handled by the ClientConnection.
			switch (frame.Command)
			{
				case StompCommand.Send:
					HandleSendCommand(frame);
					break;

				case StompCommand.Subscribe:
					HandleSubscribeCommand(frame);
					break;

				case StompCommand.Unsubscribe:
					HandleUnsubscribeCommand(frame);
					break;

				case StompCommand.Begin:
					HandleBeginCommand(frame);
					break;

				case StompCommand.Commit:
					HandleCommitCommand(frame);
					break;

				case StompCommand.Abort:
					HandleAbortCommand(frame);
					break;

				case StompCommand.Ack:
					HandleAckCommand(frame);
					break;

				default:
					HandleUnknownCommand(frame);
					break;
			}
		}

		private void HandleSendCommand(StompFrame frame)
		{
			var destination = frame.Headers[StompHeader.Destination];
			if (destination == null)
			{
				const string message = "Missing " + StompHeader.Destination + " in " + StompCommand.Send + " command";
				var errorFrame = StompFrameUtils.CreateErrorFrame(message, frame);
				SendFrame(errorFrame);
				return;
			}

			var publish = false;

			if (destination.StartsWith(QueueName.PublishPrefix))
			{
				destination = destination.Substring(QueueName.PublishPrefix.Length);
				publish = true;
			}
			else if (destination.StartsWith(QueueName.QueuePrefix))
			{
				destination = destination.Substring(QueueName.QueuePrefix.Length);
			}

			var messageQueue = _serverData.FindMessageQueue(destination);

			// TODO: we could probably get away without creating a copy, and modifying the frame
			// Don't allocate the message-id here, as the message queue does it.
			var messageFrame = StompFrameUtils.CreateCopy(frame);
			messageFrame.Command = StompCommand.Message;
			if (publish)
			{
				Log.Debug("Publishing message to topic: " + messageQueue.Name);
				messageQueue.PublishFrame(messageFrame);
			}
			else
			{
				Log.Debug("Adding message to queue: " + messageQueue.Name);
				messageQueue.AddFrame(messageFrame);
			}
			SendReceiptIfNecessary(frame);
			_serverData.LogSendMessage(frame, destination);
		}

		private void HandleSubscribeCommand(StompFrame frame)
		{
			var id = frame.Headers[StompHeader.Id];
			var destination = frame.Headers[StompHeader.Destination];
			var ack = frame.Headers[StompHeader.Ack];

			if (string.IsNullOrEmpty(id))
			{
				var errorFrame = StompFrameUtils.CreateMissingHeaderError(StompHeader.Id, frame);
				SendFrame(errorFrame);
				Disconnect();
				return;
			}

			if (string.IsNullOrEmpty(destination))
			{
				var errorFrame = StompFrameUtils.CreateMissingHeaderError(StompHeader.Destination, frame);
				SendFrame(errorFrame);
				Disconnect();
				return;
			}

			if (_subscriptions.ContainsKey(id))
			{
				var message = "Subscription already exists: " + id;
				var errorFrame = StompFrameUtils.CreateErrorFrame(message, frame);
				SendFrame(errorFrame);
				Disconnect();
				return;
			}

			var messageQueue = _serverData.FindMessageQueue(destination);

			var subscription = new ServerSideSubscription(this, id, messageQueue, ack);
			_subscriptions.Add(id, subscription);
			SendReceiptIfNecessary(frame);

		}

		private void HandleUnsubscribeCommand(StompFrame frame)
		{
			var id = frame.Headers[StompHeader.Id];
			if (string.IsNullOrEmpty(id))
			{
				var errorFrame = StompFrameUtils.CreateMissingHeaderError(StompHeader.Id, frame);
				SendFrame(errorFrame);
				Disconnect();
				return;
			}

			if (!_subscriptions.ContainsKey(id))
			{
				var message = "Subscription does not exist: " + id;
				var errorFrame = StompFrameUtils.CreateErrorFrame(message, frame);
				SendFrame(errorFrame);
				Disconnect();
				return;
			}

			var subscription = _subscriptions[id];
			_subscriptions.Remove(id);
			subscription.Dispose();
			SendReceiptIfNecessary(frame);
		}

		private void HandleBeginCommand(StompFrame frame)
		{
			// TODO: handle send command
			SendNotImplementedError(frame);
		}

		private void HandleCommitCommand(StompFrame frame)
		{
			// TODO: handle send command
			SendNotImplementedError(frame);
		}

		private void HandleAbortCommand(StompFrame frame)
		{
			// TODO: handle send command
			SendNotImplementedError(frame);
		}

		private void HandleAckCommand(StompFrame frame)
		{
			var subscriptionId = frame.Headers[StompHeader.Subscription];
			if (string.IsNullOrEmpty(subscriptionId))
			{
				var errorFrame = StompFrameUtils.CreateMissingHeaderError(StompHeader.Subscription, frame);
				SendFrame(errorFrame);
				Disconnect();
				return;
			}

			var messageId = frame.Headers[StompHeader.MessageId];
			if (string.IsNullOrEmpty(messageId))
			{
				var errorFrame = StompFrameUtils.CreateMissingHeaderError(StompHeader.MessageId, frame);
				SendFrame(errorFrame);
				Disconnect();
				return;
			}

			ServerSideSubscription subscription;
			if (!_subscriptions.TryGetValue(subscriptionId, out subscription))
			{
				var message = "Subscription does not exist: " + subscription;
				var errorFrame = StompFrameUtils.CreateErrorFrame(message, frame);
				SendFrame(errorFrame);
				Disconnect();
				return;
			}

			long messageId64;
			if (!long.TryParse(messageId, out messageId64))
			{
				var message = "Invalid value for " + StompHeader.MessageId + ": " + messageId;
				var errorFrame = StompFrameUtils.CreateErrorFrame(message, frame);
				SendFrame(errorFrame);
				Disconnect();
				return;
			}

			subscription.Acknowledge(messageId64);
			SendReceiptIfNecessary(frame);
		}

		private void HandleUnknownCommand(StompFrame frame)
		{
			var message = "Unknown command: " + frame.Command;
			var errorFrame = StompFrameUtils.CreateErrorFrame(message, frame);
			SendFrame(errorFrame);
			Disconnect();
		}

		private void SendNotImplementedError(StompFrame frame)
		{
			var message = "Sorry, the " + frame.Command + " has not been implemented yet.";
			var errorFrame = StompFrameUtils.CreateErrorFrame(message, frame);
			SendFrame(errorFrame);
		}

		public void SendFrame(StompFrame frame)
		{
			using (_lockObject.Lock())
			{
				if (_clientConnection != null)
				{
					_clientConnection.SendFrame(frame);
				}
			}
		}

		public void Disconnect()
		{
			using (_lockObject.Lock())
			{
				if (_clientConnection != null)
				{
					_clientConnection.Disconnect();
				}
			}
		}

		private void SendReceiptIfNecessary(StompFrame frame)
		{
			var receiptId = frame.Headers[StompHeader.Receipt];
			if (receiptId != null)
			{
				var receiptFrame = new StompFrame
				                   	{
				                   		Command = StompCommand.Receipt,
				                   		Headers =
				                   			{
				                   				{StompHeader.ReceiptId, receiptId}
				                   			}
				                   	};

				SendFrame(receiptFrame);
			}
		}
	}
}