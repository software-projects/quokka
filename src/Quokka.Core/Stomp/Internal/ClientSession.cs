using System;
using System.Collections.Generic;
using Common.Logging;
using Quokka.Diagnostics;

namespace Quokka.Stomp.Internal
{
	/// <summary>
	/// 	Wraps a connection with a STOMP client and performs session management.
	/// </summary>
	internal class ClientSession : IDisposable
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly object _lockObject = new object();
		private readonly ServerData _serverData;
		private ClientConnection _clientConnection;
		private readonly Dictionary<string, ClientSubscription> _subscriptions = new Dictionary<string, ClientSubscription>();

		public string SessionId { get; private set; }

		public ClientSession(ServerData serverData)
		{
			SessionId = Guid.NewGuid().ToString();
			_serverData = Verify.ArgumentNotNull(serverData, "serverData");
		}

		public void Dispose()
		{
			lock (_lockObject)
			{
				_clientConnection = null;
			}
		}

		public bool AddConnection(ClientConnection clientConnection)
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

			var messageQueue = _serverData.FindMessageQueue(destination);

			// TODO: we could probably get away without creating a copy, and modifying the frame
			// Don't allocate the message-id here, as the message queue does it.
			var messageFrame = StompFrameUtils.CreateCopy(frame);
			messageFrame.Command = StompCommand.Message;
			messageQueue.AddFrame(messageFrame);
			SendReceiptIfNecessary(frame);
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

			var subscription = new ClientSubscription(this, id, messageQueue, ack == "client");
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

			ClientSubscription subscription;
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
			var receiptId = frame.Headers[StompHeader.ReceiptId];
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