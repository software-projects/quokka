using System;
using System.Collections.Generic;
using System.Net;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.Sandbox;

namespace Quokka.Stomp
{
	/// <summary>
	/// 	This class allows a program to easily interact with a STOMP message broker.
	/// </summary>
	public class StompClient
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

		public void ConnectTo(EndPoint endPoint)
		{
			Verify.ArgumentNotNull(endPoint, "endPoint");
			lock (_lockObject)
			{
				if (_transport != null)
				{
					Log.Warn("ConnectTo already called");
					return;
				}

				// the only kind of transport we handle at the moment
				IPEndPoint ipEndPoint = (IPEndPoint) endPoint;

				RemoteEndPoint = endPoint;
				_transport = new StompClientTransport();
				_transport.ConnectedChanged += TransportConnectedChangedHandler;
				_transport.FrameReady += TransportFrameReadyHandler;
				_transport.TransportException += TransportExceptionHandler;
				_transport.Connect(ipEndPoint);
			}
		}

		public StompSubscription CreateSubscription(string destination)
		{
			Verify.ArgumentNotNull(destination, "destination");
			lock (_lockObject)
			{
				var subscriptionId = ++_lastSubscriptionId;
				var subscription = new StompSubscription(this, subscriptionId, destination);
				_subscriptions.Add(subscriptionId, subscription);
				subscription.StateChanged += new EventHandler(SubscriptionStateChanged);
				return subscription;
			}
		}

		public void SubscriptionStateChanged(object sender, EventArgs e)
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
				_receiptId += 1;
				message.Headers[StompHeader.Receipt] = _receiptId.ToString();
				_pendingSendMessages.Enqueue(message);
				SendNextMessage();
			}
		}

		public void SendMessage(StompFrame message)
		{
			Verify.ArgumentNotNull(message, "message");

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
					if (_transport.Connected)
					{
						var login = Login ?? string.Empty;
						var passcode = Passcode ?? string.Empty;

						// time to send a CONNECT message
						var frame = new StompFrame
						            	{
						            		Command = StompCommand.Connect,
						            		Headers =
						            			{
						            				{StompHeader.Login, login},
						            				{StompHeader.Passcode, passcode}
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
			StompFrame frame = null;

			for (;;)
			{
				lock (_lockObject)
				{
					frame = _transport.GetNextFrame();
					if (frame == null)
					{
						return;
					}

					switch (frame.Command)
					{
						case StompCommand.Connected:
							_sessionId = frame.Headers[StompHeader.Session];
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
				var id = long.Parse(sentMessage.Headers[StompHeader.Receipt]);

				if (id > receiptId)
				{
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
			if (!_sendInProgress && _connected && _pendingSendMessages.Count > 0)
			{
				var frame = _pendingSendMessages.Peek();
				_sendInProgress = true;
				_transport.SendFrame(frame);
				if (Log.IsDebugEnabled)
				{
					Log.DebugFormat("{0} command sent: {1}={2}", frame.Command, StompHeader.Receipt, frame.Headers[StompHeader.Receipt]);
				}
			}
		}
	}
}