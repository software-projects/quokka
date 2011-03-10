using System;
using System.Collections.Generic;
using System.Linq;
using Common.Logging;
using Quokka.Diagnostics;

namespace Quokka.Stomp.Internal
{
	internal class MessageQueue
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly object _lockObject = new object();
		private Node _first;
		private Node _last;
		private readonly List<ServerSideSubscription> _subscriptions = new List<ServerSideSubscription>();
		private readonly Random _random = new Random((int) DateTime.Now.Ticks);

		public string Name { get; private set; }

		public MessageQueue(string messageQueueName)
		{
			Name = Verify.ArgumentNotNull(messageQueueName, "messageQueueName");
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

				while (_first != null)
				{
					var frame = _first.Frame;
					_first = _first.Next;
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
				var subscription = ChooseSubscription();

				if (subscription != null)
				{
					// A subscription exists -- send the message straight away
					Log.Debug("Sending straight to subscription " + subscription.SubscriptionId);
					subscription.SendFrame(frame);
					return;
				}

				// The message id gets allocated when the message is removed.
				// The reason for this is that published messages get a message
				// number as well, and we don't want an ACK for a published
				// message to accidentally acknowledge a message on the queue.
				var node = new Node
				           	{
				           		Frame = frame,
				           		Previous = _last,
				           		Next = _first,
				           	};
				_last = node;

				if (_first == null)
				{
					_first = node;
				}

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

		private class Node
		{
			public StompFrame Frame;
			public Node Previous;
			public Node Next;
		}
	}
}