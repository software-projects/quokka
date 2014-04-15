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
using System.Threading;
using Quokka.Diagnostics;
using Quokka.Stomp;
using Quokka.Stomp.Internal;

namespace Quokka.Sprocket
{
	public partial class SprocketClient
	{
		private class Channel : IChannel
		{
			private readonly SprocketClient _sprocket;
			private readonly StompSubscription _subscription;
			private readonly string _queueName = Guid.NewGuid().ToString();
			private bool _isDisposed;
			private readonly LockObject _lock;
			private readonly Dictionary<Type, Action<object>> _actions = new Dictionary<Type, Action<object>>();
			private readonly Timer _timer;
			private bool _timerStarted;

			public Channel(SprocketClient sprocket)
			{
				_sprocket = Verify.ArgumentNotNull(sprocket, "client");
				_lock = sprocket.Client.Lock;
				SynchronizationContext = _sprocket.SynchronizationContext;
				_subscription = _sprocket.Client.CreateSubscription(_queueName);
				_subscription.MessageArrived += SubscriptionMessageArrived;
				_subscription.Subscribe();
				_timer = new Timer(TimerCallback);
				Timeout = TimeSpan.MaxValue;
			}

			public void Dispose()
			{
				using (_lock.Lock())
				{
					if (_isDisposed)
					{
						_isDisposed = true;
						_subscription.Dispose();
						_timer.Dispose();
					}
				}
			}

			public ISprocket Sprocket
			{
				get { return _sprocket; }
			}

			public SynchronizationContext SynchronizationContext { get; set; }

			public TimeSpan Timeout { get; set; }

			public Action TimeoutAction { get; set; }

			public void Send(object message)
			{
				using (_lock.Lock())
				{
					if (_isDisposed)
					{
						throw new ObjectDisposedException("Channel");
					}

					if (message == null)
					{
						return;
					}

					var type = message.GetType();
					var fullName = type.FullName;
					var destination = QueueName.QueuePrefix + fullName;

					var frame = new StompFrame(StompCommand.Send)
					            	{
					            		Headers =
					            			{
					            				{StompHeader.Destination, destination},
					            				{StompHeader.NonStandard.ReplyTo, _queueName},
					            			}
					            	};

					if (Timeout < TimeSpan.MaxValue && Timeout > TimeSpan.Zero)
					{
						var expires = DateTimeOffset.Now + Timeout;
						frame.Headers[StompHeader.NonStandard.Expires] = ExpiresTextUtils.ToString(expires);
						_timer.Change(Timeout, TimeSpan.FromMilliseconds(-1));
						_timerStarted = true;
					}

					frame.Serialize(message);
					_sprocket.Client.SendMessage(frame);
				}
			}

			public IChannel HandleResponse<T>(Action<T> action)
			{
				using (_lock.Lock())
				{
					if (_isDisposed)
					{
						throw new ObjectDisposedException("Channel");
					}
					var adapter = new ActionAdapter<T>(action, this);
					_actions[typeof (T)] = adapter.DoAction;
					return this;
				}
			}

			private void TimerCallback(object state)
			{
				using (_lock.Lock())
				{
					if (!_isDisposed && _timerStarted)
					{
						_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
						_timerStarted = false;

						if (TimeoutAction != null)
						{
							var action = TimeoutAction;
							_lock.AfterUnlock(() => InvokeAction(action));
						}
					}
				}
			}

			private void InvokeAction(Action action)
			{
				if (SynchronizationContext == null)
				{
					action();
				}
				else
				{
					SynchronizationContext.Send(delegate { action(); }, null);
				}
			}

			private void SubscriptionMessageArrived(object sender, StompMessageEventArgs e)
			{
				using (_lock.Lock())
				{
					if (_isDisposed)
					{
						return;
					}

					// turn off timer
					_timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
					_timerStarted = false;

					var frame = e.Message;
					object message = frame.Deserialize();
					var type = message.GetType();

					Action<object> action;
					if (!_actions.TryGetValue(type, out action))
					{
						type = typeof (object);
						if (!_actions.TryGetValue(type, out action))
						{
							// TODO: could not figure out what to do with this message
						}
					}

					if (action != null)
					{
						_lock.AfterUnlock(() => action(message));
					}
				}
			}

			private class ActionAdapter<T>
			{
				private readonly Action<T> _action;
				private readonly Channel _channel;

				public ActionAdapter(Action<T> action, Channel channel)
				{
					_action = action;
					_channel = channel;
				}

				public void DoAction(object obj)
				{
					var synchronizationContext = _channel.SynchronizationContext;
					var t = (T) obj;
					if (synchronizationContext == null)
					{
						_action(t);
					}
					else
					{
						synchronizationContext.Send(delegate { _action(t); }, null);
					}
				}
			}
		}
	}
}