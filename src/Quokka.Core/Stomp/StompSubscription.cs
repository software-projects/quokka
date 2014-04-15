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
using System.Threading;
using Castle.Core.Logging;
using Quokka.Diagnostics;
using Quokka.Stomp.Internal;

namespace Quokka.Stomp
{
	///<summary>
	///	Represents a subscription registered with the STOMP server.
	///</summary>
	public class StompSubscription : IDisposable
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private readonly LockObject _lock;
		private readonly string _subscriptionIdText;

		public StompClient Client { get; private set; }
		public int SubscriptionId { get; private set; }
		public string Destination { get; private set; }
		public StompSubscriptionState State { get; private set; }
		public SynchronizationContext SynchronizationContext { get; set; }
		public string Ack { get; set; }

		public event EventHandler<StompMessageEventArgs> MessageArrived;
		public event EventHandler StateChanged;

		internal StompSubscription(StompClient client, LockObject lockObject, int subscriptionId, string destination)
		{
			Client = Verify.ArgumentNotNull(client, "client");
			_lock = Verify.ArgumentNotNull(lockObject, "lockObject");
			Destination = Verify.ArgumentNotNull(destination, "destination");
			SubscriptionId = subscriptionId;
			Ack = StompAck.Auto;

			// minor optimisation: because we are sending this a lot as a string, convert
			// to string once and resend it.
			_subscriptionIdText = subscriptionId.ToString();
		}

		public void SubscriptionLost()
		{
			using (_lock.Lock())
			{
				if (State == StompSubscriptionState.Subscribed
					|| State == StompSubscriptionState.Subscribing)
				{
					State = StompSubscriptionState.Unsubscribed;
					_lock.AfterUnlock(RaiseStateChanged);
				}
			}
		}

		public void Subscribe()
		{
			using (_lock.Lock())
			{
				if (State == StompSubscriptionState.Unsubscribed)
				{
					var message = new StompFrame(StompCommand.Subscribe)
					              	{
					              		Headers =
					              			{
					              				{StompHeader.Id, _subscriptionIdText},
					              				{StompHeader.Destination, Destination},
					              				{StompHeader.Ack, Ack},
					              			}
					              	};
					_lock.AfterUnlock(RaiseStateChanged);
					State = StompSubscriptionState.Subscribing;
					Client.SendRawMessage(message, true);
				}
			}
		}

		public void Dispose()
		{
			using (_lock.Lock())
			{
				var oldState = State;
				if (!IsDisposed())
				{
					State = StompSubscriptionState.Disposed;
					_lock.AfterUnlock(RaiseStateChanged);
					if (oldState == StompSubscriptionState.Subscribing || oldState == StompSubscriptionState.Subscribed)
					{
						var message = new StompFrame(StompCommand.Unsubscribe)
						              	{
						              		Headers =
						              			{
						              				{StompHeader.Id, _subscriptionIdText}
						              			}
						              	};
						Client.SendRawMessage(message, false);
					}
				}
			}
			MessageArrived = null;
			StateChanged = null;
		}

		internal void ReceiveMessage(StompFrame message)
		{
			using (_lock.Lock())
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
					if (Ack != StompAck.Auto)
					{
						var ackMessage = new StompFrame(StompCommand.Ack)
						                 	{
						                 		Headers =
						                 			{
						                 				{StompHeader.Subscription, _subscriptionIdText},
						                 				{StompHeader.MessageId, messageId},
						                 			}
						                 	};
						Client.SendRawMessage(ackMessage, false);
					}
				}
			}

			RaiseMessageArrived(message);
		}

		internal void Confirm()
		{
			using (_lock.Lock())
			{
				if (State == StompSubscriptionState.Subscribing)
				{
					State = StompSubscriptionState.Subscribed;
					_lock.AfterUnlock(RaiseStateChanged);
				}
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
				var synchronizationContext = SynchronizationContext;
				if (synchronizationContext == null)
				{
					StateChanged(this, EventArgs.Empty);
				}
				else
				{
					SendOrPostCallback callback =(obj => StateChanged(this, EventArgs.Empty));
					synchronizationContext.Send(callback, null);
				}
			}
		}

		private void RaiseMessageArrived(StompFrame frame)
		{
			if (MessageArrived != null)
			{
				var args = new StompMessageEventArgs(frame);
				var synchronizationContext = SynchronizationContext;
				if (synchronizationContext == null)
				{
					MessageArrived(this, args);
				}
				else
				{
					SendOrPostCallback callback = (obj => MessageArrived(this, args));
					synchronizationContext.Send(callback, null);
				}
			}
		}
	}
}