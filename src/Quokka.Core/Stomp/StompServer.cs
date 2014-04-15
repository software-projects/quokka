using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Castle.Core.Logging;
using Quokka.Diagnostics;
using Quokka.Stomp.Internal;
using Quokka.Stomp.Transport;

namespace Quokka.Stomp
{
	public class StompServer : IDisposable
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private readonly List<IListener<StompFrame>> _listeners = new List<IListener<StompFrame>>();
		private readonly List<EndPoint> _listenEndPoints = new List<EndPoint>();
		private readonly List<ServerSideConnection> _clientConnections = new List<ServerSideConnection>();
		private readonly object _lockObject = new object();
		private readonly ServerData _serverData = new ServerData();
		private bool _isDisposed;

		public IList<EndPoint> EndPoints
		{
			get { return new ReadOnlyCollection<EndPoint>(_listenEndPoints); }
		}

		public StompServerConfig Config
		{
			get { return _serverData.Config; }
		}

		public void Dispose()
		{
			lock (_lockObject)
			{
				_isDisposed = true;
				foreach (var listener in _listeners)
				{
					listener.Dispose();
				}

				foreach (var connection in _clientConnections)
				{
					connection.Disconnect();
				}
			}
		}

		public void ListenOn(EndPoint endPoint)
		{
			lock (_lockObject)
			{
				if (_isDisposed)
				{
					throw new ObjectDisposedException("StompServer");
				}
				var ipEndPoint = endPoint as IPEndPoint;
				if (ipEndPoint == null)
				{
					throw new ArgumentException("Cannot listen on endpoint: " + endPoint);
				}

				var listener = new StompListener {SpecifiedEndPoint = ipEndPoint};

				listener.ListenException += ListenerListenException;
				listener.ClientConnected += ListenerClientConnected;
				listener.StartListening();

				_listeners.Add(listener);
				_listenEndPoints.Add(listener.ListenEndPoint);

				// if this is the first endpoint added, start the cleanup timer
				if (_listeners.Count == 1)
				{
					_serverData.StartCleanupTimer();
				}
			}
		}

		private void ListenerClientConnected(object sender, EventArgs e)
		{
			var newConnections = new List<ServerSideConnection>();

			Log.Debug("STOMP Client connected");
			lock (_lockObject)
			{
				if (_isDisposed)
				{
					return;
				}
				var listener = (IListener<StompFrame>) sender;
				for (;;)
				{
					var transport = listener.GetNextTransport();
					if (transport == null)
					{
						break;
					}

					var clientConnection = new ServerSideConnection(transport, _serverData);
					clientConnection.ConnectionClosed += ClientConnectionClosed;

					// Perform a check here because the transport could have disconnected before
					// we had a chance to subscribe to the ConnectionClosed event of its connection.
					if (transport.Connected)
					{
						_clientConnections.Add(clientConnection);
						newConnections.Add(clientConnection);
					}
					else
					{
						Log.Warn("Client connection closed immediately after connection");
					}
				}
			}

			// These objects might have received frames before they had a chance to subscribe to events
			foreach (var clientConnection in newConnections)
			{
				clientConnection.ProcessReceivedFrames();
			}
		}

		private void ClientConnectionClosed(object sender, EventArgs e)
		{
			lock (_lockObject)
			{
				if (_isDisposed)
				{
					return;
				}
				var clientConnection = (ServerSideConnection) sender;
				_clientConnections.Remove(clientConnection);
			}
		}

		private static void ListenerListenException(object sender, ExceptionEventArgs e)
		{
			Log.Error("Listener exception: " + e.Exception.Message, e.Exception);
		}
	}
}