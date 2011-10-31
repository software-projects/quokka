using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using Common.Logging;
using Quokka.Diagnostics;
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
		private readonly Timer _outgoingHeartBeatTimer;
		private readonly Timer _incomingHeartBeatTimer;
		private readonly Timer _connectTimer;
		private bool _connected;
		private bool _waitingForConnectedFrame;
		private HeartBeatValues _sentHeartBeatValues;
		private HeartBeatValues _negotiatedHeartBeatValues;
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
		public EndPoint RemoteEndPoint { get; private set; }

		/// <summary>
		///		Outgoing heart beat timeout in milliseconds
		/// </summary>
		public int OutgoingHeartBeat { get; set; }

		/// <summary>
		///		Incoming heart beat timeout in milliseconds
		/// </summary>
		public int IncomingHeartBeat { get; set; }

		/// <summary>
		///		
		/// </summary>
		public TimeSpan ConnectTimeout { get; set; }


		public StompClient()
		{
			Login = string.Empty;
			Passcode = string.Empty;

			OutgoingHeartBeat = 30000;
			IncomingHeartBeat = 30000;
			ConnectTimeout = TimeSpan.FromSeconds(15);

			_outgoingHeartBeatTimer = new Timer(OutgoingHeartBeatTimerCallback);
			_incomingHeartBeatTimer = new Timer(IncomingHeartBeatTimerCallback);
			_connectTimer = new Timer(ConnectTimerCallback);
		}

		public void Dispose()
		{
			lock (_lockObject)
			{
				if (!_isDisposed)
				{
					_isDisposed = true;
					UnsubscribeTransportEvents();
					if (_transport != null)
					{
						_transport.Shutdown();
						_transport.Dispose();
						_transport = null;
					}
					_incomingHeartBeatTimer.Dispose();
					_outgoingHeartBeatTimer.Dispose();
					_connectTimer.Dispose();
					_connected = false;
					_waitingForConnectedFrame = false;
				}
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

		private void DisconnectAndReconnect()
		{
			lock (_lockObject)
			{
				if (_transport != null)
				{
					UnsubscribeTransportEvents();
					_transport.Dispose();
					_transport = null;
				}
				_waitingForConnectedFrame = false;
				_sendInProgress = false;
			}

			// call this without a lock, as it has its own lock and
			// releases it for callbacks
			CheckConnected();

			lock (_lockObject)
			{
				if (_transport == null)
				{
					_transport = new StompClientTransport();
					SubscribeTransportEvents();
					_transport.Connect((IPEndPoint)RemoteEndPoint);
				}
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

		internal void SendRawMessage(StompFrame message, bool receiptRequired)
		{
			lock (_lockObject)
			{
				if (!_isDisposed)
				{
					if (receiptRequired)
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
			SendRawMessage(message, false);
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
			SendRawMessage(frame, false);
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
			CheckConnected();
		}

		private void CheckConnected() {

			bool raiseConnectedChanged = false;

			lock (_lockObject)
			{
				if (!_isDisposed)
				{
					if (_connected)
					{
						if (_transport == null || !_transport.Connected)
						{
							_connected = false;
							Log.Debug("Client is now disconnected from server");
							raiseConnectedChanged = true;
						}
						StopOutgoingHeartBeatTimer();
						StopIncomingHeartBeatTimer();
						StopConnectTimer();
					}
					else
					{
						if (_transport != null && _transport.Connected && !_waitingForConnectedFrame)
						{
							var login = Login ?? string.Empty;
							var passcode = Passcode ?? string.Empty;

							// the client id provides some help with diagnostics by identifying the client process
							var processName = Path.GetFileNameWithoutExtension(Process.GetCurrentProcess().MainModule.FileName) ;
							var clientId = Environment.MachineName
							               + "/" + processName
							               + "/" + Process.GetCurrentProcess().Id;

							_sentHeartBeatValues = new HeartBeatValues(OutgoingHeartBeat, IncomingHeartBeat);

							// time to send a CONNECT message
							var frame = new StompFrame
							            	{
							            		Command = StompCommand.Connect,
							            		Headers =
							            			{
							            				{StompHeader.Login, login},
							            				{StompHeader.Passcode, passcode},
														{StompHeader.HeartBeat, _sentHeartBeatValues.ToString()},
														{StompHeader.NonStandard.ClientId, clientId}
							            			}
							            	};

							// a little enhancement, if we are re-connecting a previous session,
							// include the session id so that we can resume
							if (_sessionId != null)
							{
								frame.Headers[StompHeader.Session] = _sessionId;
							}

							_waitingForConnectedFrame = true;
							StartConnectTimer();
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

					// received a frame, so we can restart the timer
					StartIncomingHeartBeatTimer();

					if (frame.IsHeartBeat)
					{
						// received a heart beat frame -- all we want to do is restart the timer
						continue;
					}

					switch (frame.Command)
					{
						case StompCommand.Connected:
							connectedChanged = true;
							HandleConnected(frame);
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

		private void HandleConnected(StompFrame frame)
		{
			AssignSessionAndResubscribe(frame);
			StopConnectTimer();
			_connected = true;
			_waitingForConnectedFrame = false;
			Log.DebugFormat("Received {0} response, {1}={2}", frame.Command, StompHeader.Session, _sessionId);

			var serverHeartBeatValues = new HeartBeatValues(frame.Headers[StompHeader.HeartBeat]);
			_negotiatedHeartBeatValues= _sentHeartBeatValues.CombineWith(serverHeartBeatValues);

			StartIncomingHeartBeatTimer();
			StartOutgoingHeartBeatTimer();
			SendNextMessage();
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

            if (_pendingSendMessages.Count == 0)
            {
                Log.ErrorFormat("Received RECEIPT {0} but nothing asked for it", receiptId);
                _transport.Shutdown();
                return;
            }

            var sentMessage = _pendingSendMessages.Peek();
            var idText = sentMessage.Headers[StompHeader.Receipt];
            if (idText == null)
            {
                Log.ErrorFormat("Received RECEIPT {0} but nothing asked for it (and there is a message queued)",
                               receiptId);
                _transport.Shutdown();
                return;
            }

            // the current implementation never sends more than one message requiring receipt without receiving a receipt
            var id = long.Parse(sentMessage.Headers[StompHeader.Receipt]);
            if (id != receiptId)
            {
                Log.ErrorFormat("Received RECEIPT {0} but expected RECEIPT {1}", id, receiptId);
                _transport.Shutdown();
            }

            _pendingSendMessages.Dequeue();
            _sendInProgress = false;

            if (sentMessage.Command == StompCommand.Subscribe)
            {
                int subscriptionId = int.Parse(sentMessage.Headers[StompHeader.Id]);
                StompSubscription subscription;
                if (_subscriptions.TryGetValue(subscriptionId, out subscription))
                {
                    subscription.Confirm();
                }
            }

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
					StartOutgoingHeartBeatTimer();
				}
				catch (InvalidOperationException)
				{
					// TODO this is far from ideal -- need to modify the API to ITransport
					Log.Warn("Attempt to send frame after transport shutdown");
					return;
				}
				if (frame.Headers[StompHeader.Receipt] == null)
				{
					// we do not want a receipt
					if (Log.IsDebugEnabled)
					{
						Log.DebugFormat("{0} command sent: no receipt required", frame.Command);
					}
					_pendingSendMessages.Dequeue();
				}
				else
				{
					// this is a frame for which we want a receipt so stop sending
					// and wait for the RECEIPT command from the server
					_sendInProgress = true;
					if (Log.IsDebugEnabled)
					{
						Log.DebugFormat("{0} command sent: {1}={2}", frame.Command, StompHeader.Receipt, frame.Headers[StompHeader.Receipt]);
					}
				}
			}
		}

		private void AssignSessionAndResubscribe(StompFrame frame)
		{
			var oldSessionId = _sessionId;
			_sessionId = frame.Headers[StompHeader.Session];
			if (oldSessionId != _sessionId && _subscriptions.Count > 0)
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

		private void StartConnectTimer()
		{
			int milliseconds = (int)ConnectTimeout.TotalMilliseconds;
			_connectTimer.Change(milliseconds, Timeout.Infinite);
		}

		private void StartOutgoingHeartBeatTimer()
		{
			if (_negotiatedHeartBeatValues.Outgoing > 0)
			{
				_outgoingHeartBeatTimer.Change(_negotiatedHeartBeatValues.Outgoing, Timeout.Infinite);
			}
		}

		private void StartIncomingHeartBeatTimer()
		{
			if (_negotiatedHeartBeatValues.Incoming > 0)
			{
				_incomingHeartBeatTimer.Change(_negotiatedHeartBeatValues.Incoming*2, Timeout.Infinite);
			}
		}

		private void StopOutgoingHeartBeatTimer()
		{
			_outgoingHeartBeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private void StopIncomingHeartBeatTimer()
		{
			_outgoingHeartBeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private void StopConnectTimer()
		{
			_connectTimer.Change(Timeout.Infinite, Timeout.Infinite);
		}

		private void OutgoingHeartBeatTimerCallback(object state)
		{
			lock (_lockObject)
			{
				if (!_isDisposed && Connected)
				{
					SendRawMessage(StompFrame.HeartBeat, false);
				}
			}
		}

		private void IncomingHeartBeatTimerCallback(object state)
		{
			Log.Error("Timeout waiting for heart-beat from server");
			DisconnectAndReconnect();
		}

		private void ConnectTimerCallback(object state)
		{
			Log.Error("Timeout waiting for " + StompCommand.Connected + " response from server");
			DisconnectAndReconnect();
		}
	}
}