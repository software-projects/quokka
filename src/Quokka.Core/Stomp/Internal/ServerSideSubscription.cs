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
		private readonly LockObject _lockObject = GlobalLock.LockObject;
		private readonly Dictionary<long, StompFrame> _unacknowledgedFrames = new Dictionary<long, StompFrame>();

		public ServerSideSubscription(ServerSideSession session, string subscriptionId, MessageQueue messageQueue,
		                              string ack)
		{
			Session = Verify.ArgumentNotNull(session, "session");
			SubscriptionId = Verify.ArgumentNotNull(subscriptionId, "subscriptionId");
			MessageQueue = Verify.ArgumentNotNull(messageQueue, "messageQueue");
			MessageQueue.AddSubscription(this);
			AutoAcknowledge = ack == StompAck.Auto;

			MessageQueue.MessageReceived += MessageQueueMessageReceived;
			MessageQueue.MessagePublished += MessageQueueMessagePublished;
		}

		public void ReceiveMessagesFromQueue()
		{
			while (Session.IsConnected)
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

		private void MessageQueueMessageReceived(object sender, EventArgs e)
		{
			ReceiveMessagesFromQueue();
		}

		public void Dispose()
		{
			MessageQueue.RemoveSubscription(this);
			MessageQueue.MessageReceived -= MessageQueueMessageReceived;
			MessageQueue.MessagePublished -= MessageQueueMessagePublished;
		}

		public void SendFrame(StompFrame frame)
		{
			using (_lockObject.Lock())
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
			using (_lockObject.Lock())
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
			using (_lockObject.Lock())
			{
				status.UnacknowledgedFrameCount = _unacknowledgedFrames.Count;
				status.TotalMessageCount = _lastMessageId;
			}
			return status;
		}
	}
}