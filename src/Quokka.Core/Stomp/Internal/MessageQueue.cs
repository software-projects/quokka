using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.Stomp.Server.Messages;

namespace Quokka.Stomp.Internal
{
	internal class MessageQueue : IDisposable
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly object _lockObject = new object();
		private readonly List<ServerSideSubscription> _subscriptions = new List<ServerSideSubscription>();
		private readonly Random _random = new Random((int) DateTime.Now.Ticks);
		private readonly LinkedList<StompFrame> _linkedList = new LinkedList<StompFrame>();
		private long _totalMessageCount;
		private bool _isDisposed;

		public string Name { get; private set; }
		public bool IsDisposed
		{
			get { return _isDisposed; }
		}

		public MessageQueue(string messageQueueName)
		{
			Name = Verify.ArgumentNotNull(messageQueueName, "messageQueueName");
		}

		public void Dispose()
		{
			lock (_lockObject)
			{
				// Does nothing at present. Would be useful if the queues were persisted.
				_isDisposed = true;
			}
		}

		public bool IsUnused
		{
			get
			{
				lock (_lockObject)
				{
					return _subscriptions.Count == 0 && _linkedList.Count == 0;
				}
			}
		}

		public MessageQueueStatus CreateStatus()
		{
			var status = new MessageQueueStatus {MessageQueueName = Name};
			lock (_lockObject)
			{
				status.SubscriptionCount = _subscriptions.Count;
				status.PendingMessageCount = _linkedList.Count;
				status.TotalMessageCount = _totalMessageCount;
			}
			return status;
		}

		public void RemoveExpired()
		{
			lock (_lockObject)
			{
				// Minor optimisation: convert current time to a text string because
				// it is quicker to compare strings than to constantly convert date/times
				// to and from strings.
				var utcNow = ExpiresTextUtils.ToString(DateTimeOffset.Now);

				var removeNodes = new List<LinkedListNode<StompFrame>>();
				for (var node = _linkedList.First; node != null; node = node.Next)
				{
					if (node.Value.IsExpiredAt(utcNow)) 
					{
						removeNodes.Add(node);
					}
				}

				foreach (var node in removeNodes)
				{
					Log.Debug("Removing expired message from queue " + Name);
					_linkedList.Remove(node);
				}
			}
		}

		public void AddSubscription(ServerSideSubscription subscription)
		{
			lock (_lockObject)
			{
				_subscriptions.Add(subscription);

				// Send all the queued messages to the subscription. This is not ideal,
				// because it does not cope with the case where there are multiple clients
				// connecting for the same queue -- the first client will get all the messages
				// queued up, and the second client will get none. This is not something that
				// will be done in the near future with this library, so let it go for now.
				// Needs a bit of a re-think of the relationship between subscriptions and
				// the message queue.
				//
				// Probably need change it so that the message queue gives the message
				// to the subscription a couple at a time, and then the subscription asks 
				// for more messages once they have been acknowledged by the client.

				while (_linkedList.Count > 0)
				{
					var frame = _linkedList.First.Value;
					_linkedList.RemoveFirst();
					subscription.SendFrame(frame);
				}
			}
		}

		public void RemoveSubscription(ServerSideSubscription subscription)
		{
			lock (_lockObject)
			{
				_subscriptions.Remove(subscription);
			}
		}

		private ServerSideSubscription ChooseSubscription()
		{
			if (_subscriptions.Count == 0)
			{
				return null;
			}
			if (_subscriptions.Count == 1)
			{
				return _subscriptions.First();
			}

			int index = _random.Next(_subscriptions.Count);
			return _subscriptions[index];
		}

		public void AddFrame(StompFrame frame)
		{
			lock (_lockObject)
			{
				++_totalMessageCount;
				var subscription = ChooseSubscription();

				if (subscription != null)
				{
					// A subscription exists -- send the message straight away
					Log.Debug("Sending straight to subscription " + subscription.SubscriptionId);
					subscription.SendFrame(frame);
					return;
				}

				_linkedList.AddLast(frame);
				Log.Debug("Added to end of queue");
			}
		}

		/// <summary>
		/// 	Sends the frame to all currently published subscribers.
		/// </summary>
		public void PublishFrame(StompFrame frame)
		{
			lock (_lockObject)
			{
				++_totalMessageCount;
				if (_subscriptions.Count == 0)
				{
					// no subscriptions
					return;
				}

				for (int index = 1; index < _subscriptions.Count; ++index)
				{
					// Because each subscription gets to modify the frame,
					// the second and subsequent subscriptions (if any) 
					// need to have a copy.
					var copy = StompFrameUtils.CreateCopy(frame);
					_subscriptions[index].SendFrame(copy);
				}

				// Minor optimisation: publish the message to the first subscription 
				// without taking a copy
				_subscriptions.First().SendFrame(frame);
			}
		}
	}
}