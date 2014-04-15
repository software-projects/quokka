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
using Quokka.Stomp;

namespace Quokka.Sprocket
{
	public partial class SprocketClient
	{
		private class Subscriber<T> : ISubscriber<T>
		{
			private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
			private readonly SprocketClient _sprocket;
			private readonly StompSubscription _subscription;
			private volatile bool _isDisposed;
			private Action<T> _action;

			public Subscriber(SprocketClient sprocket)
			{
				_sprocket = sprocket;
				SynchronizationContext = _sprocket.SynchronizationContext;
				var destination = SubscriptionType.FullName;
				_subscription = _sprocket.Client.CreateSubscription(destination);
				_subscription.MessageArrived += SubscriptionMessageArrived;

				// Do not call the Subscribe method on _subscription just yet, otherwise
				// we could get messages without having an action. The Subscribe method
				// is called as soon as a non-null value is assigned to the Action property.
			}

			public void Dispose()
			{
				_isDisposed = true;
				_subscription.Dispose();
				_action = null;
			}

			public Type SubscriptionType
			{
				get { return typeof (T); }
			}

			public SynchronizationContext SynchronizationContext { get; set; }


			public Action<T> Action
			{
				get { return _action; }
				set
				{
					_action = value;
					if (_subscription.State == StompSubscriptionState.Unsubscribed
					    && _action != null)
					{
						// now that we have an action, we are prepared to subscribe
						_subscription.Subscribe();
					}
				}
			}

			public Func<T, bool> Filter { get; set; }

			private class Message
			{
				public StompFrame Frame;
				public object Payload;
			}

			private void SubscriptionMessageArrived(object sender, StompMessageEventArgs e)
			{
				if (_isDisposed)
				{
					return;
				}

				var frame = e.Message;

				if (!frame.CanDeserialize())
				{
					string message = string.Format("Received frame that cannot be deserialized");
					Log.Error(message);
					return;
				}

				T payload;
				try
				{
					payload = (T) frame.Deserialize();
				}
				catch (Exception ex)
				{
					Log.Error("Error deserializing payload of type " + typeof (T) + ": " + ex.Message, ex);
					return;
				}

				var filter = Filter;
				if (filter != null)
				{
					if (!filter(payload))
					{
						return;
					}
				}

				var sc = SynchronizationContext;
				if (sc == null)
				{
					PerformAction(frame, payload);
				}
				else
				{
					sc.Send(delegate { PerformAction(frame, payload); }, null);
				}
			}

			private void PerformAction(StompFrame frame, T payload)
			{
				var action = Action;
				if (action == null)
				{
					Log.Warn("Message received for subscriber but no action defined");
					return;
				}

				try
				{
					CurrentMessage.Frame = frame;
					action(payload);
				}
				catch (Exception ex)
				{
					Log.Error("Unexpected error handling subscriber message: " + ex.Message, ex);
				}
				finally
				{
					CurrentMessage.Frame = null;
				}
			}
		}
	}
}