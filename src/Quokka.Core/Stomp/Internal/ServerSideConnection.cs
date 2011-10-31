using System;
using System.Threading;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.Stomp.Transport;

namespace Quokka.Stomp.Internal
{
	internal class ServerSideConnection
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		private readonly ITransport<StompFrame> _transport;
		private readonly ServerData _serverData;
		private readonly object _lockObject = GlobalLock.Instance;

		private delegate void StateAction(StompFrame frame);

		private StateAction _stateAction;
		private ServerSideSession _session;
		private HeartBeatValues _negotiatedHeartBeats;
		private readonly Timer _incomingHeartBeatTimer;
		private readonly Timer _outgoingHeartBeatTimer;
		private Timer _connectTimer;

		public event EventHandler ConnectionClosed;

		public ServerSideConnection(ITransport<StompFrame> transport, ServerData serverData)
		{
			_transport = Verify.ArgumentNotNull(transport, "transport");
			_serverData = Verify.ArgumentNotNull(serverData, "serverData");
			_transport.ConnectedChanged += TransportConnectedChanged;
			_transport.FrameReady += TransportFrameReady;
			_transport.TransportException += TransportTransportException;
			_stateAction = ExpectingConnect;
			_incomingHeartBeatTimer = new Timer(HandleIncomingHeartBeatTimeout);
			_outgoingHeartBeatTimer = new Timer(HandleOutgoingHeartBeatTimeout);
			_connectTimer = new Timer(HandleConnectTimeout, null,
			                          (int) serverData.Config.ConnectFrameTimeout.TotalMilliseconds,
			                          Timeout.Infinite);
		}

		public void SendFrame(StompFrame frame)
		{
			lock (_lockObject)
			{
				if (_stateAction == ShuttingDown)
				{
					Log.Warn("Discarded frame: transport is shutting down: " + frame);
				}
				else
				{
					_transport.SendFrame(frame);
					StartOutgoingHeartBeatTimer();
				}
			} 
		}

		public void Disconnect()
		{
			lock (_lockObject)
			{
				DisconnectWithoutLocking();
			}
		}

		private void DisconnectWithoutLocking()
		{
			if (_stateAction == ShuttingDown)
			{
				Log.Warn("Ignoring duplicate Disconnect request");
			}
			else
			{
				_transport.Shutdown();
				_stateAction = ShuttingDown;
				_outgoingHeartBeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
				_incomingHeartBeatTimer.Change(Timeout.Infinite, Timeout.Infinite);
				if (_connectTimer != null)
				{
					_connectTimer.Dispose();
					_connectTimer = null;
				}
			}
		}

		// ReSharper disable MemberCanBeMadeStatic.Local
		// ReSharper disable UnusedParameter.Local
		private bool Authenticate(string login, string passcode)
		{
			// TODO: perform authentication here. Possibly call out to an external service.
			return true;
		}
		// ReSharper restore UnusedParameter.Local
		// ReSharper restore MemberCanBeMadeStatic.Local

		private void ExpectingConnect(StompFrame frame)
		{
			if (frame.Command != StompCommand.Connect
				&& frame.Command != StompCommand.Stomp)
			{
				string message = "Expecting " + StompCommand.Connected 
					+ " or " + StompCommand.Stomp
					+ " command, received " + frame.Command;
				Log.Error(message);
				var errorFrame = StompFrameUtils.CreateErrorFrame(message);
				_transport.SendFrame(errorFrame);
				DisconnectWithoutLocking();
				return;
			}

			var login = frame.Headers[StompHeader.Login];
			var passcode = frame.Headers[StompHeader.Passcode];

			if (!Authenticate(login, passcode))
			{
				string message = "Received " + frame.Command + " frame, Access denied";
				Log.Warn(message);
				var errorFrame = StompFrameUtils.CreateErrorFrame(message);
				_transport.SendFrame(errorFrame);
				DisconnectWithoutLocking();
				return;
			}

			Log.Debug("Received " + frame.Command + " frame, authenticated OK");

			var sessionId = frame.Headers[StompHeader.Session];
			ServerSideSession session = null;

			if (sessionId != null)
			{
				session = _serverData.FindSession(sessionId);
				if (session == null)
				{
					Log.Warn("Received " + frame.Command + " frame for non-existent session: " + sessionId);
					var message = ErrorMessages.SessionDoesNotExistPrefix + sessionId;
					Log.Debug(message);
					var errorFrame = StompFrameUtils.CreateErrorFrame(message);
					_transport.SendFrame(errorFrame);
					DisconnectWithoutLocking();
					return;
				}

				if (!session.AddConnection(this))
				{
					var message = frame.Command + " frame requested a session already in use: " + sessionId;
					Log.Warn(message);
					var errorFrame = StompFrameUtils.CreateErrorFrame(message);
					_transport.SendFrame(errorFrame);
					DisconnectWithoutLocking();
					return;
				}

				Log.Debug("Reconnected to session " + sessionId);
			}

			if (session == null)
			{
				session = _serverData.CreateSession();
				session.AddConnection(this);
				Log.Debug("Created new session " + session.SessionId);
			}

			// helps with debugging if we give the session a more friendly name
			session.ClientId = frame.Headers[StompHeader.NonStandard.ClientId]; 
			_session = session;

			var connectedFrame = new StompFrame
			                     	{
			                     		Command = StompCommand.Connected,
			                     		Headers =
			                     			{
			                     				{StompHeader.Session, session.SessionId}
			                     			}
			                     	};

			if (frame.Headers[StompHeader.HeartBeat] == null)
			{
				// no heart-beat header received, so we default to 0,0
				_negotiatedHeartBeats = new HeartBeatValues(0, 0);
			}
			else
			{
				var otherHeartBeatValues = new HeartBeatValues(frame.Headers[StompHeader.HeartBeat]);
				var myHeartBeatValues = new HeartBeatValues(30000, 30000);
				_negotiatedHeartBeats = myHeartBeatValues.CombineWith(otherHeartBeatValues);
				connectedFrame.Headers[StompHeader.HeartBeat] = _negotiatedHeartBeats.ToString();
			}
			_transport.SendFrame(connectedFrame);
			if (_connectTimer != null)
			{
				_connectTimer.Change(Timeout.Infinite, Timeout.Infinite);
				_connectTimer.Dispose();
				_connectTimer = null;
			}
			StartIncomingHeartBeatTimer();
			StartOutgoingHeartBeatTimer();
			_stateAction = Connected;
			Log.Debug("Session " + session + " connected");
		}

		private void Connected(StompFrame frame)
		{
			lock (_lockObject)
			{
				StartIncomingHeartBeatTimer();
				if (frame.IsHeartBeat)
				{
					// nothing else to do
				}
				else if (frame.Command == StompCommand.Disconnect)
				{
					DisconnectWithoutLocking();
					var keepSession = frame.GetBoolean(StompHeader.NonStandard.KeepSession, false);
					if (!keepSession)
					{
						_serverData.EndSession(_session);
					}
					_session = null;
				}
				else
				{
					try
					{
						// ReSharper disable PossibleNullReferenceException
						_session.ProcessFrame(frame);
						// ReSharper restore PossibleNullReferenceException
					}
					catch (Exception ex)
					{
						if (ex.IsCorruptedStateException())
						{
							throw;
						}
						Log.Error("Unexpected error handling " + frame.Command + " frame: " + ex.Message, ex);

						try
						{
							var errorFrame = StompFrameUtils.CreateErrorFrame("internal server error", frame);
							_transport.SendFrame(errorFrame);
							DisconnectWithoutLocking();
						}
						catch (InvalidOperationException)
						{
							// Not ideal, but we can encounter a situation where the transport is shutting down, so if
							// we send anything it is going to throw and exception. Because we currently do not have an
							// API for checking this, we just handle the exception.
						}
					}
				}
			}
		}

		private static void ShuttingDown(StompFrame frame)
		{
			Log.WarnFormat("Discarded {0} message as connection is shutting down", frame.Command);
		}

		private void TransportConnectedChanged(object sender, EventArgs e)
		{
			if (_transport.Connected)
			{
				Log.Debug("Transport connected");
			}
			else
			{
				Log.Debug("Transport disconnected");
				if (ConnectionClosed != null)
				{
					ConnectionClosed(this, EventArgs.Empty);
				}
			}
		}

		private void TransportFrameReady(object sender, EventArgs e)
		{
			// Don't lock, as the called method does its own locking.
			ProcessReceivedFrames();
		}

		public void ProcessReceivedFrames()
		{
			for (;;)
			{
				var frame = _transport.GetNextFrame();
				if (frame == null)
				{
					return;
				}

				lock (_lockObject)
				{
					try
					{
						_stateAction(frame);
					}
					catch (Exception ex)
					{
						Log.Error("Unexpected exception: " + ex.Message, ex);
						if (_transport.Connected)
						{
							var errorFrame = StompFrameUtils.CreateErrorFrame("Internal server error");
							_transport.SendFrame(errorFrame);
							DisconnectWithoutLocking();
						}
					}
				}
			}
		}

		private static void TransportTransportException(object sender, ExceptionEventArgs e)
		{
			Log.Error("Transport layer exception: " + e.Exception.Message, e.Exception);
		}

		private void StartIncomingHeartBeatTimer()
		{
			if (_negotiatedHeartBeats.Incoming > 0)
			{
				// allow for twice as long as the negotiated value
				var timeout = _negotiatedHeartBeats.Incoming*2;
				_incomingHeartBeatTimer.Change(timeout, Timeout.Infinite);
			}
		}

		private void StartOutgoingHeartBeatTimer()
		{
			if (_negotiatedHeartBeats.Outgoing > 0)
			{
				_outgoingHeartBeatTimer.Change(_negotiatedHeartBeats.Outgoing, Timeout.Infinite);
			}
		}

		private void HandleIncomingHeartBeatTimeout(object state)
		{
			lock (_lockObject)
			{
				if (_stateAction == Connected)
				{
					Log.Warn("Incoming connection timed out, disconnecting");
					DisconnectWithoutLocking();
				}
			}
		}

		private void HandleOutgoingHeartBeatTimeout(object state)
		{
			lock (_lockObject)
			{
				if (_stateAction == Connected)
				{
					SendFrame(StompFrame.HeartBeat);
					StartOutgoingHeartBeatTimer();
				}
			}
		}

		private void HandleConnectTimeout(object state)
		{
			lock (_lockObject)
			{
				if (_connectTimer != null && _stateAction == ExpectingConnect)
				{
					const string message = "Timed out waiting for " + StompCommand.Connect + " frame";
					Log.Warn(message);
					var errorFrame = StompFrameUtils.CreateErrorFrame(message);
					_transport.SendFrame(errorFrame);
					DisconnectWithoutLocking();
				}
			}
		}
	}
}