using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.Stomp.Server.Messages;

namespace Quokka.Stomp.Internal
{
	internal class MessageQueue : IDisposable
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly object _lockObject = new object();
		// it's not really necessary to have a list of subscriptions here anymore -- it could be removed pretty easily
		private readonly List<ServerSideSubscription> _subscriptions = new List<ServerSideSubscription>();
		private readonly LinkedList<StompFrame> _linkedList = new LinkedList<StompFrame>();
		private long _totalMessageCount;
		private bool _isDisposed;

		public string Name { get; private set; }
		public bool IsDisposed
		{
			get { return _isDisposed; }
		}

		public event EventHandler MessageReceived;
		public event EventHandler<StompMessageEventArgs> MessagePublished;

		public MessageQueue(string messageQueueName)
		{
			Name = Verify.ArgumentNotNull(messageQueueName, "messageQueueName");

			// Ensure the events are subscribed, so that we do not have to check for
			// null before raising the events. There is a race condition between the
			// check for null and raising the event.
			MessageReceived += delegate { };
			MessagePublished += delegate { };
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
			}
		}

		public void RemoveSubscription(ServerSideSubscription subscription)
		{
			lock (_lockObject)
			{
				_subscriptions.Remove(subscription);
			}
		}

		public void AddFrame(StompFrame frame)
		{
			lock (_lockObject)
			{
				++_totalMessageCount;
				_linkedList.AddLast(frame);
			}

			ThreadPool.QueueUserWorkItem(delegate { MessageReceived(this, EventArgs.Empty); });
		}

		public StompFrame RemoveFrame()
		{
			StompFrame frame = null;
			lock (_lockObject)
			{
				if (_linkedList.Count > 0)
				{
					frame = _linkedList.First();
					_linkedList.RemoveFirst();
				}
			}
			return frame;
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
			}

			ThreadPool.QueueUserWorkItem(delegate { MessagePublished(this, new StompMessageEventArgs(frame)); });
		}
	}
}