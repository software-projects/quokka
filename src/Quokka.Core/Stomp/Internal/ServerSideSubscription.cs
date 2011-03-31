using System;
using System.Collections.Generic;
using Quokka.Diagnostics;
using Quokka.Stomp.Server.Messages;

namespace Quokka.Stomp.Internal
{
	/// <summary>
	/// 	Represents a client subscription to a destination.
	/// </summary>
	internal class ServerSideSubscription : IDisposable
	{
		public string SubscriptionId { get; private set; }
		public ServerSideSession Session { get; private set; }
		public MessageQueue MessageQueue { get; private set; }
		public bool AutoAcknowledge { get; private set; }
		private long _lastMessageId;
		private long _lastAcknowledgedMessageId;
		private readonly object _lockObject = GlobalLock.Instance;
		private readonly Dictionary<long, StompFrame> _unacknowledgedFrames = new Dictionary<long, StompFrame>();

		public ServerSideSubscription(ServerSideSession session, string subscriptionId, MessageQueue messageQueue,
		                              string ack)
		{
			Session = Verify.ArgumentNotNull(session, "session");
			SubscriptionId = Verify.ArgumentNotNull(subscriptionId, "subscriptionId");
			MessageQueue = Verify.ArgumentNotNull(messageQueue, "messageQueue");
			MessageQueue.AddSubscription(this);
			AutoAcknowledge = ack == StompAck.Auto;

			MessageQueue.MessageReceived += (o, e) => ReceiveMessagesFromQueue();
			MessageQueue.MessagePublished += MessageQueueMessagePublished;
		}

		public void ReceiveMessagesFromQueue()
		{
			// This method does not lock
			for (; ; )
			{
				var frame = MessageQueue.RemoveFrame();
				if (frame == null)
				{
					return;
				}
				SendFrame(frame);
			}
		}

		private void MessageQueueMessagePublished(object sender, StompMessageEventArgs e)
		{
			// This method does not lock
			var frame = StompFrameUtils.CreateCopy(e.Message);
			SendFrame(frame);
		}

		public void Dispose()
		{
			MessageQueue.RemoveSubscription(this);
		}

		public void SendFrame(StompFrame frame)
		{
			lock (_lockObject)
			{
				var messageId = ++_lastMessageId;
				frame.Headers[StompHeader.MessageId] = messageId.ToString();
				frame.Headers[StompHeader.Subscription] = SubscriptionId;
				if (!AutoAcknowledge)
				{
					_unacknowledgedFrames.Add(messageId, frame);
				}
			}

			Session.SendFrame(frame);
		}

		public void Acknowledge(long messageId)
		{
			lock (_lockObject)
			{
				for (long id = _lastAcknowledgedMessageId + 1; id <= messageId; ++id)
				{
					_unacknowledgedFrames.Remove(id);
				}
				_lastAcknowledgedMessageId = messageId;
			}
		}

		public SubscriptionStatus CreateStatus()
		{
			var status = new SubscriptionStatus
			             	{
			             		SubscriptionId = SubscriptionId,
			             		AutoAcknowledge = AutoAcknowledge,
			             		MessageQueueName = MessageQueue.Name,
			             	};
			lock (_lockObject)
			{
				status.UnacknowledgedFrameCount = _unacknowledgedFrames.Count;
				status.TotalMessageCount = _lastMessageId;
			}
			return status;
		}
	}
}