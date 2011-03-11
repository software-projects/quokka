using System;
using Common.Logging;
using Quokka.Diagnostics;

namespace Quokka.Stomp
{
	///<summary>
	///	Represents a subscription registered with the STOMP server.
	///</summary>
	public class StompSubscription : IDisposable
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly object _lockObject = new object();
		private readonly string _subscriptionIdText;

		public StompClient Client { get; private set; }
		public int SubscriptionId { get; private set; }
		public string Destination { get; private set; }
		public StompSubscriptionState State { get; private set; }

		public event EventHandler<StompMessageEventArgs> MessageArrived;
		public event EventHandler StateChanged;

		internal StompSubscription(StompClient client, int subscriptionId, string destination)
		{
			Client = Verify.ArgumentNotNull(client, "client");
			Destination = Verify.ArgumentNotNull(destination, "destination");
			SubscriptionId = subscriptionId;

			// minor optimisation: because we are sending this a lot as a string, convert
			// to string once and resend it.
			_subscriptionIdText = subscriptionId.ToString();
		}

		public void Subscribe()
		{
			var raiseStateChanged = false;

			lock (_lockObject)
			{
				if (State == StompSubscriptionState.Unsubscribed)
				{
					var message = new StompFrame(StompCommand.Subscribe)
					              	{
					              		Headers =
					              			{
					              				{StompHeader.Id, _subscriptionIdText},
					              				{StompHeader.Destination, Destination},
					              				{StompHeader.Ack, StompAck.Client},
					              			}
					              	};
					raiseStateChanged = true;
					State = StompSubscriptionState.Subscribing;
					Client.SendRawMessage(message);
				}
			}

			if (raiseStateChanged)
			{
				RaiseStateChanged();
			}
		}

		public void Dispose()
		{
			var raiseStateChanged = false;

			lock (_lockObject)
			{
				var oldState = State;
				if (!IsDisposed())
				{
					State = StompSubscriptionState.Disposed;
					raiseStateChanged = true;
					if (oldState == StompSubscriptionState.Subscribing || oldState == StompSubscriptionState.Subscribed)
					{
						var message = new StompFrame(StompCommand.Unsubscribe)
						              	{
						              		Headers =
						              			{
						              				{StompHeader.Id, _subscriptionIdText}
						              			}
						              	};
						Client.SendRawMessage(message);
					}
				}
			}

			if (raiseStateChanged)
			{
				RaiseStateChanged();
			}
		}

		internal void ReceiveMessage(StompFrame message)
		{
			lock (_lockObject)
			{
				if (IsDisposed())
				{
					// Silently discard messages received after being disposed.
					// By not acknowledging them, they will not be discarded by the server
					return;
				}

				var messageId = message.Headers[StompHeader.MessageId];

				if (messageId == null)
				{
					Log.Warn("Received message from server without a " + StompHeader.MessageId + " header");
				}
				else
				{
					var ackMessage = new StompFrame(StompCommand.Ack)
					                 	{
					                 		Headers =
					                 			{
					                 				{StompHeader.Subscription, _subscriptionIdText},
					                 				{StompHeader.MessageId, messageId},
					                 			}
					                 	};
					Client.SendRawMessage(ackMessage);
				}
			}

			// I know this is not thread-safe, but the usage pattern for this event is to create the
			// subscription, subscribe to the event and stay subscribed to the event.
			if (MessageArrived != null)
			{
				MessageArrived(this, new StompMessageEventArgs(message));
			}
		}

		internal void Confirm()
		{
			var raiseStateChanged = false;

			lock (_lockObject)
			{
				if (State == StompSubscriptionState.Subscribing)
				{
					State = StompSubscriptionState.Subscribed;
					raiseStateChanged = true;
				}
			}

			if (raiseStateChanged)
			{
				RaiseStateChanged();
			}
		}

		private bool IsDisposed()
		{
			return State == StompSubscriptionState.Disposed;
		}

		private void RaiseStateChanged()
		{
			if (StateChanged != null)
			{
				StateChanged(this, EventArgs.Empty);
			}
		}
	}
}