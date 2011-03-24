using System;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.Sandbox;
using Quokka.Stomp.Transport;

namespace Quokka.Stomp.Internal
{
	internal class ServerSideConnection
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		private readonly ITransport<StompFrame> _transport;
		private readonly ServerData _serverData;
		private readonly object _lockObject = new object();

		private delegate void StateAction(StompFrame frame);

		private StateAction _stateAction;
		private ServerSideSession _session;

		public event EventHandler ConnectionClosed;

		public ServerSideConnection(ITransport<StompFrame> transport, ServerData serverData)
		{
			_transport = Verify.ArgumentNotNull(transport, "transport");
			_serverData = Verify.ArgumentNotNull(serverData, "serverData");
			_transport.ConnectedChanged += TransportConnectedChanged;
			_transport.FrameReady += TransportFrameReady;
			_transport.TransportException += TransportTransportException;
			_stateAction = ExpectingConnect;
		}

		public void SendFrame(StompFrame frame)
		{
			if (Log.IsDebugEnabled)
			{
				Log.DebugFormat("Sent {0} command to client", frame.Command);
			}
			_transport.SendFrame(frame);
		}

		public void Disconnect()
		{
			_transport.Shutdown();
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
			if (frame.Command != StompCommand.Connect)
			{
				const string message = "Expecting " + StompCommand.Connected + " command";
				Log.Debug(message);
				var errorFrame = StompFrameUtils.CreateErrorFrame(message);
				_transport.SendFrame(errorFrame);
				_transport.Shutdown();
				_stateAction = ShuttingDown;
				return;
			}

			var login = frame.Headers[StompHeader.Login];
			var passcode = frame.Headers[StompHeader.Passcode];

			if (!Authenticate(login, passcode))
			{
				const string message = "Access denied";
				Log.Debug(message);
				var errorFrame = StompFrameUtils.CreateErrorFrame(message);
				_transport.SendFrame(errorFrame);
				_transport.Shutdown();
				_stateAction = ShuttingDown;
				return;
			}

			var sessionId = frame.Headers[StompHeader.Session];
			ServerSideSession session = null;

			if (sessionId != null)
			{
				session = _serverData.FindSession(sessionId);
				if (session == null)
				{
					var message = ErrorMessages.SessionDoesNotExistPrefix + sessionId;
					Log.Debug(message);
					var errorFrame = StompFrameUtils.CreateErrorFrame(message);
					_transport.SendFrame(errorFrame);
					_transport.Shutdown();
					_stateAction = ShuttingDown;
					return;
				}

				if (!session.AddConnection(this))
				{
					var message = "Session already in use: " + sessionId;
					Log.Debug(message);
					var errorFrame = StompFrameUtils.CreateErrorFrame(message);
					_transport.SendFrame(errorFrame);
					_transport.Shutdown();
					_stateAction = ShuttingDown;
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
			_transport.SendFrame(connectedFrame);
			_stateAction = Connected;
			Log.Debug("Session " + session + " connected");
		}

		private void Connected(StompFrame frame)
		{
			Log.Debug("Received frame " + frame.Command);
			lock (_lockObject)
			{
				if (frame.Command == StompCommand.Disconnect)
				{
					_stateAction = ShuttingDown;
					_transport.Shutdown();

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
							_transport.Shutdown();
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
						_transport.Shutdown();
					}
				}
			}
		}

		private static void TransportTransportException(object sender, ExceptionEventArgs e)
		{
			Log.Error("Transport layer exception: " + e.Exception.Message, e.Exception);
		}
	}
}