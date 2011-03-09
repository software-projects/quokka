using System;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.Sandbox;

namespace Quokka.Stomp.Internal
{
	internal class ClientConnection
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

		private readonly ITransport<StompFrame> _transport;
		private readonly ServerData _serverData;
		private readonly object _lockObject = new object();

		private delegate void StateAction(StompFrame frame);

		private StateAction _stateAction;
		private ClientSession _clientSession;

		public event EventHandler ConnectionClosed;

		public ClientConnection(ITransport<StompFrame> transport, ServerData serverData)
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
				var errorFrame = StompFrameUtils.CreateErrorFrame("Access denied");
				_transport.SendFrame(errorFrame);
				_transport.Shutdown();
				_stateAction = ShuttingDown;
				return;
			}

			var sessionId = frame.Headers[StompHeader.Session];
			ClientSession clientSession = null;

			if (sessionId != null)
			{
				clientSession = _serverData.FindSession(sessionId);
				if (clientSession == null)
				{
					var errorFrame = StompFrameUtils.CreateErrorFrame("Session does not exist: " + sessionId);
					_transport.SendFrame(errorFrame);
					_transport.Shutdown();
					_stateAction = ShuttingDown;
					return;
				}
			}

			if (clientSession == null)
			{
				clientSession = _serverData.CreateSession();
			}

			_clientSession = clientSession;

			var connectedFrame = new StompFrame
			                     	{
			                     		Command = StompCommand.Connected,
			                     		Headers =
			                     			{
			                     				{StompHeader.Session, clientSession.SessionId}
			                     			}
			                     	};
			_transport.SendFrame(connectedFrame);
			_stateAction = Connected;
		}

		private void Connected(StompFrame frame)
		{
			lock (_lockObject)
			{
				if (frame.Command == StompCommand.Disconnect)
				{
					_stateAction = ShuttingDown;
					_transport.Shutdown();
					_serverData.EndSession(_clientSession);
					_clientSession = null;
				}
				try
				{
					// ReSharper disable PossibleNullReferenceException
					_clientSession.ProcessFrame(frame);
					// ReSharper restore PossibleNullReferenceException
				}
				catch (Exception ex)
				{
					if (ex.IsCorruptedStateException())
					{
						throw;
					}
					Log.Error("Unexpected error handling " + frame.Command + " frame: " + ex.Message, ex);

					var errorFrame = StompFrameUtils.CreateErrorFrame("internal server error", frame);
					_transport.SendFrame(errorFrame);
					_transport.Shutdown();
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