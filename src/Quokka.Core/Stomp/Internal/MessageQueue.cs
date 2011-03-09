using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Quokka.Diagnostics;

namespace Quokka.Stomp.Internal
{
	internal class MessageQueue
	{
		private readonly object _lockObject = new object();
		private Node _first;
		private Node _last;
		private readonly List<ClientSubscription> _subscriptions = new List<ClientSubscription>();
		private readonly Random _random = new Random((int)DateTime.Now.Ticks);

		public string Name { get; private set; }

		public MessageQueue(string messageQueueName)
		{
			Name = Verify.ArgumentNotNull(messageQueueName, "messageQueueName");
		}

		public void AddSubscription(ClientSubscription clientSubscription)
		{
			lock (_lockObject)
			{
				_subscriptions.Add(clientSubscription);
			}
		}

		public void RemoveSubscription(ClientSubscription clientSubscription)
		{
			lock (_lockObject)
			{
				_subscriptions.Remove(clientSubscription);
			}
		}

		private ClientSubscription ChooseSubscription()
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
					StompFrameUtils.AllocateMessageId(frame);
					subscription.SendFrame(frame);
					return;
				}

				// The message id gets allocated when the message is removed.
				// The reason for this is that published messages get a message
				// number as well, and we don't want an ACK for a published
				// message to accidentally acknowledge a message on the queue.
				var node = new Node
				           	{
				           		MessageId = 0,
				           		Frame = frame,
				           		Previous = _last,
				           		Next = _first,
				           	};
				_last = node;

				if (_first == null)
				{
					_first = node;
				}
			}
		}

		/// <summary>
		/// Sends the frame to all currently published subscribers.
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

		// Get the next message, if any, after the specified message id
		public StompFrame GetNext(long messageId)
		{
			lock (_lockObject)
			{
				for (var node = _first; node != null; node = node.Next)
				{
					if (messageId <= 0 || node.MessageId > messageId)
					{
						if (node.MessageId == 0)
						{
							node.MessageId = StompFrameUtils.AllocateMessageId(node.Frame);
						}
						return node.Frame;
					}
				}
			}

			return null;
		}

		public StompFrame GetNextAndAcknowledge()
		{
			lock (_lockObject)
			{
				if (_first != null)
				{
					var frame = _first.Frame;
					_first = _first.Next;
					return frame;
				}
			}
			return null;
		}

		// Acknowledge all messages up to and including the message id
		public void Acknowledge(long messageId)
		{
			lock (_lockObject)
			{
				while (_first != null)
				{
					if (_first.MessageId > messageId)
					{
						// front message is not being acknowledged
						return;
					}
					_first = _first.Next;
				}
			}
		}

		private class Node
		{
			public long MessageId;
			public StompFrame Frame;
			public Node Previous;
			public Node Next;
		}

	}
}
