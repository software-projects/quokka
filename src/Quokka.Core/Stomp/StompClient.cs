using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.Sandbox;
using Quokka.Stomp.Internal;
using Quokka.Stomp.Transport;

namespace Quokka.Stomp
{
	/// <summary>
	/// 	This class allows a program to easily interact with a STOMP message broker.
	/// </summary>
	public class StompClient : IDisposable
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly object _lockObject = new object();
		private StompClientTransport _transport;
		private readonly Queue<StompFrame> _pendingSendMessages = new Queue<StompFrame>();
		private readonly Dictionary<int, StompSubscription> _subscriptions = new Dictionary<int, StompSubscription>();
		private bool _connected;
		private string _sessionId;
		private long _receiptId;
		private bool _sendInProgress;
		private int _lastSubscriptionId;
		private bool _isDisposed;

		///<summary>
		///	This event is raised whenever the value of the <see cref = "Connected" /> property changes.
		///</summary>
		public event EventHandler ConnectedChanged;

		///<summary>
		///	Login to use when connecting to the STOMP message broker.
		///</summary>
		public string Login { get; set; }

		///<summary>
		///	Passcode to use when connecting to the STOMP message broker.
		///</summary>
		public string Passcode { get; set; }

		/// <summary>
		/// 	The endpoint of the STOMP message broker
		/// </summary>
		public EndPoint RemoteEndPoint { get; set; }

		public StompClient()
		{
			Login = string.Empty;
			Passcode = string.Empty;
		}

		public void Dispose()
		{
			_isDisposed = true;
			UnsubscribeTransportEvents();
			if (_transport != null)
			{
				_transport.Shutdown();
				_transport.Dispose();
				_transport = null;
			}
		}

		public bool Connected
		{
			get
			{
				lock (_lockObject)
				{
					return _connected;
				}
			}
		}

		private void SubscribeTransportEvents()
		{
			if (_transport != null)
			{
				_transport.ConnectedChanged += TransportConnectedChangedHandler;
				_transport.FrameReady += TransportFrameReadyHandler;
				_transport.TransportException += TransportExceptionHandler;
			}
		}

		private void UnsubscribeTransportEvents()
		{
			if (_transport != null)
			{
				_transport.ConnectedChanged += TransportConnectedChangedHandler;
				_transport.FrameReady += TransportFrameReadyHandler;
				_transport.TransportException += TransportExceptionHandler;
			}
		}

		private void CheckDisposed()
		{
			if (_isDisposed)
			{
				throw new ObjectDisposedException("StompClient");
			}
		}

		public void ConnectTo(EndPoint endPoint)
		{
			Verify.ArgumentNotNull(endPoint, "endPoint");
			lock (_lockObject)
			{
				CheckDisposed();
				if (_transport != null)
				{
					Log.Warn("ConnectTo already called");
					return;
				}

				// the only kind of transport we handle at the moment
				IPEndPoint ipEndPoint = (IPEndPoint) endPoint;

				RemoteEndPoint = endPoint;
				_transport = new StompClientTransport();
				SubscribeTransportEvents();
				_transport.Connect(ipEndPoint);
			}
		}

		public StompSubscription CreateSubscription(string destination)
		{
			Verify.ArgumentNotNull(destination, "destination");
			lock (_lockObject)
			{
				CheckDisposed();
				var subscriptionId = ++_lastSubscriptionId;
				var subscription = new StompSubscription(this, subscriptionId, destination);
				_subscriptions.Add(subscriptionId, subscription);
				subscription.StateChanged += SubscriptionStateChanged;
				return subscription;
			}
		}

		private void SubscriptionStateChanged(object sender, EventArgs e)
		{
			var subscription = (StompSubscription) sender;
			if (subscription.State == StompSubscriptionState.Disposed)
			{
				lock (_lockObject)
				{
					_subscriptions.Remove(subscription.SubscriptionId);
				}
			}
		}

		internal void SendRawMessage(StompFrame message)
		{
			lock (_lockObject)
			{
				if (!_isDisposed)
				{
					if (message.Command != StompCommand.Ack)
					{
						_receiptId += 1;
						message.Headers[StompHeader.Receipt] = _receiptId.ToString();
					}
					_pendingSendMessages.Enqueue(message);
					SendNextMessage();
				}
			}
		}

		public void SendMessage(StompFrame message)
		{
			Verify.ArgumentNotNull(message, "message");
			CheckDisposed();

			if (message.Command == null)
			{
				message.Command = StompCommand.Send;
			}
			else if (message.Command != StompCommand.Send)
			{
				throw new InvalidOperationException("Only " + StompCommand.Send + " commmands permitted");
			}

			if (message.Headers[StompHeader.Destination] == null)
			{
				throw new InvalidOperationException(("Header mising: " + StompHeader.Destination));
			}
			SendRawMessage(message);
		}

		public void SendTextMessage(string destination, string text)
		{
			Verify.ArgumentNotNull(destination, "destination");
			Verify.ArgumentNotNull(text, "text");
			CheckDisposed();
			var frame = new StompFrame(StompCommand.Send)
			            	{
			            		Headers =
			            			{
			            				{StompHeader.Destination, destination},
			            				{StompHeader.ContentType, "text/plain"}
			            			},
			            		BodyText = text
			            	};
			SendRawMessage(frame);
		}

		protected virtual void OnConnectedChanged(EventArgs e)
		{
			if (ConnectedChanged != null)
			{
				ConnectedChanged(this, e);
			}
		}

		private void TransportConnectedChangedHandler(object sender, EventArgs e)
		{
			bool raiseConnectedChanged = false;

			lock (_lockObject)
			{
				if (!_isDisposed)
				{
					if (_connected)
					{
						if (!_transport.Connected)
						{
							_connected = false;
							Log.Debug("Client is now disconnected from server");
							raiseConnectedChanged = true;
						}
					}
					else
					{
						if (_transport != null && _transport.Connected)
						{
							var login = Login ?? string.Empty;
							var passcode = Passcode ?? string.Empty;
							var processName = Assembly.GetEntryAssembly() == null ? "-" : Assembly.GetEntryAssembly().GetName().Name;
							var clientId = Environment.MachineName
							               + "/" + processName
							               + "/" + Process.GetCurrentProcess().Id;

							// time to send a CONNECT message
							var frame = new StompFrame
							            	{
							            		Command = StompCommand.Connect,
							            		Headers =
							            			{
							            				{StompHeader.Login, login},
							            				{StompHeader.Passcode, passcode},
														{StompHeader.NonStandard.ClientId, clientId}
							            			}
							            	};

							// a little enhancement, if we are re-connecting a previous session,
							// include the session id so that we can resume
							if (_sessionId != null)
							{
								frame.Headers[StompHeader.Session] = _sessionId;
							}

							_transport.SendFrame(frame);
							Log.Debug("Sent " + frame.Command + " command to server");
						}
					}
				}
			}

			if (raiseConnectedChanged)
			{
				OnConnectedChanged(EventArgs.Empty);
			}
		}

		private static void TransportExceptionHandler(object sender, ExceptionEventArgs e)
		{
			Log.Error("Transport error: " + e.Exception.Message, e.Exception);
		}

		private void TransportFrameReadyHandler(object sender, EventArgs e)
		{
			ProcessNextFrame();
		}

		private void ProcessNextFrame()
		{
			var connectedChanged = false;
			StompSubscription subscription = null;
			StompFrame frame;

			for (;;)
			{
				lock (_lockObject)
				{
					if (_transport == null)
					{
						return;
					}

					frame = _transport.GetNextFrame();
					if (frame == null)
					{
						return;
					}

					switch (frame.Command)
					{
						case StompCommand.Connected:
							AssignSessionAndResubscribe(frame);
							_connected = true;
							connectedChanged = true;
							Log.DebugFormat("Received {0} response, {1}={2}", frame.Command, StompHeader.Session, _sessionId);
							SendNextMessage();
							break;
						case StompCommand.Message:
							subscription = HandleMessage(frame);
							break;
						case StompCommand.Error:
							HandleError(frame);
							break;
						case StompCommand.Receipt:
							HandleReceipt(frame);
							break;
						default:
							Log.Error("Received unexpected command from server: " + frame.Command);
							break;
					}
				}

				// Always raise the events without having a lock in place
				if (connectedChanged)
				{
					OnConnectedChanged(EventArgs.Empty);
				}
				if (subscription != null)
				{
					subscription.ReceiveMessage(frame);
				}
			}
		}

		private StompSubscription HandleMessage(StompFrame message)
		{
			var subscriptionIdText = message.Headers[StompHeader.Subscription];
			if (subscriptionIdText == null)
			{
				Log.WarnFormat("Received {0} message without a {1} header", message.Command, StompHeader.Subscription);
				return null;
			}

			int subscriptionId;
			if (!int.TryParse(subscriptionIdText, out subscriptionId))
			{
				Log.WarnFormat("Received {0} message with invalid {1}: {2}", message.Command, StompHeader.Subscription,
				               subscriptionIdText);
				return null;
			}

			StompSubscription subscription;
			if (!_subscriptions.TryGetValue(subscriptionId, out subscription))
			{
				Log.WarnFormat("Receive {0} message with unknown {1}: {2}",
				               message.Command,
				               StompHeader.Subscription,
				               subscriptionId);
				return null;
			}

			return subscription;
		}

		private void HandleError(StompFrame message)
		{
			var messageText = message.Headers[StompHeader.Message];
			if (messageText.StartsWith(ErrorMessages.SessionDoesNotExistPrefix) && !_connected)
			{
				// We have tried to reconnect to our old session, but it no longer exists, so remove it.
				Log.Info("Server says our session no longer exists. Will ask for a new one");
				_sessionId = null;
				foreach (var subscription in _subscriptions.Values)
				{
					subscription.SubscriptionLost();
				}
			}
		}

		private void HandleReceipt(StompFrame message)
		{
			var receiptIdText = message.Headers[StompHeader.ReceiptId];
			if (Log.IsDebugEnabled)
			{
				Log.DebugFormat("{0} received, {1}={2}", message.Command, StompHeader.ReceiptId, receiptIdText);
			}
			if (receiptIdText == null)
			{
				// TODO: what do we do here, disconnect?
				Log.ErrorFormat("Missing {0} header in {1} command", StompHeader.ReceiptId, message.Command);
				_transport.Shutdown();
				return;
			}

			long receiptId;
			if (!long.TryParse(receiptIdText, out receiptId))
			{
				// TODO: what to we do here, disconnect?
				Log.ErrorFormat("Invalid value for {0} header: {1}", StompHeader.ReceiptId, receiptIdText);
				_transport.Shutdown();
				return;
			}

			while (_pendingSendMessages.Count > 0)
			{
				var sentMessage = _pendingSendMessages.Peek();
				var idText = sentMessage.Headers[StompHeader.Receipt];
				if (idText == null)
				{
					// message that does not need a receipt, so remove it
					_pendingSendMessages.Dequeue();
					continue;
				}

				var id = long.Parse(sentMessage.Headers[StompHeader.Receipt]);
				if (id > receiptId)
				{
					// this message has not been acknowledged
					break;
				}

				_pendingSendMessages.Dequeue();

				if (sentMessage.Command == StompCommand.Subscribe)
				{
					int subscriptionId = int.Parse(sentMessage.Headers[StompHeader.Id]);
					StompSubscription subscription;
					if (_subscriptions.TryGetValue(subscriptionId, out subscription))
					{
						subscription.Confirm();
					}
				}
			}
			_sendInProgress = false;
			SendNextMessage();
		}

		private void SendNextMessage()
		{
			while (!_sendInProgress 
				&& _connected && 
				_pendingSendMessages.Count > 0 
				&& _transport != null)
			{
				var frame = _pendingSendMessages.Peek();
				try
				{
					_transport.SendFrame(frame);
				}
				catch (InvalidOperationException)
				{
					// TODO this is far from ideal -- need to modify the API to ITransport
					Log.Warn("Attempt to send frame after transport shutdown");
					return;
				}
				if (Log.IsDebugEnabled)
				{
					Log.DebugFormat("{0} command sent: {1}={2}", frame.Command, StompHeader.Receipt, frame.Headers[StompHeader.Receipt]);
				}
				if (frame.Headers[StompHeader.Receipt] == null)
				{
					// we do not want a receipt
					Log.Debug("No receipt required, remove from pending queue");
					_pendingSendMessages.Dequeue();
				}
				else
				{
					// this is a frame for which we want a receipt so stop sending
					// and wait for the RECEIPT command from the server
					_sendInProgress = true;
				}
			}
		}

		private void AssignSessionAndResubscribe(StompFrame frame)
		{
			var oldSessionId = _sessionId;
			_sessionId = frame.Headers[StompHeader.Session];
			if (oldSessionId == null && _subscriptions.Count > 0)
			{
				// This is where we had an old session, but lost it. The most probable
				// cause is that the server was stopped and restarted. What we do here
				// is re-send our subscription information.
				foreach (var subscription in _subscriptions.Values)
				{
					Log.DebugFormat("Resubscribing subscription {0}: {1}", subscription.SubscriptionId, subscription.Destination);
					subscription.Subscribe();
				}
			}
		}
	}
}