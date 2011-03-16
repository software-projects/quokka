using System;
using System.Collections.Generic;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.Stomp.Server.Messages;

namespace Quokka.Stomp.Internal
{
	/// <summary>
	/// 	Wraps a connection with a STOMP client and performs session management.
	/// </summary>
	internal class ServerSideSession : IDisposable
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly object _lockObject = new object();
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
			lock (_lockObject)
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
			lock (_lockObject)
			{
				status.Connected = _clientConnection != null;
				foreach (var subscription in _subscriptions.Values)
				{
					status.Subscriptions.Add(subscription.CreateStatus());
				}
			}
			return status;
		}

		public bool IsUnused
		{
			get
			{
				lock (_lockObject)
				{
					if (_clientConnection == null)
					{
						if (DateTime.Now >= _expiresAt)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public override string ToString()
		{
			return SessionId;
		}

		public bool AddConnection(ServerSideConnection clientConnection)
		{
			Verify.ArgumentNotNull(clientConnection, "clientConnection");
			lock (_lockObject)
			{
				if (_clientConnection != null)
				{
					return false;
				}
				_clientConnection = clientConnection;
				_clientConnection.ConnectionClosed += ClientConnectionClosed;
				return true;
			}
		}

		private void ClientConnectionClosed(object sender, EventArgs e)
		{
			lock (_lockObject)
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
			ProcessFrameByCommand(frame);
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

			var subscription = new ServerSideSubscription(this, id, messageQueue, ack == "client");
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
			if (_clientConnection != null)
			{
				_clientConnection.SendFrame(frame);
			}
		}

		public void Disconnect()
		{
			if (_clientConnection != null)
			{
				_clientConnection.Disconnect();
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