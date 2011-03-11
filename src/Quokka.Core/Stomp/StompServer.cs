using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Common.Logging;
using Quokka.Sandbox;
using Quokka.Stomp.Internal;

namespace Quokka.Stomp
{
	public class StompServer : IDisposable
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private readonly List<IListener<StompFrame>> _listeners = new List<IListener<StompFrame>>();
		private readonly List<EndPoint> _listenEndPoints = new List<EndPoint>();
		private readonly List<ServerSideConnection> _clientConnections = new List<ServerSideConnection>();
		private readonly object _lockObject = new object();
		private readonly ServerData _serverData = new ServerData();

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
			foreach (var listener in _listeners)
			{
				listener.Dispose();
			}

			foreach (var connection in _clientConnections)
			{
				connection.Disconnect();
			}
		}

		public void ListenOn(EndPoint endPoint)
		{
			lock (_lockObject)
			{
				var ipEndPoint = endPoint as IPEndPoint;
				if (ipEndPoint == null)
				{
					throw new ArgumentException("Cannot listen on endpoint: " + endPoint);
				}

				var listener = new StompListener {ListenEndPoint = ipEndPoint};

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
			Log.Debug("STOMP Client connected");
			lock (_lockObject)
			{
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
					}
					else
					{
						Log.Warn("Client connection closed immediately after connection");
					}
				}
			}
		}

		private void ClientConnectionClosed(object sender, EventArgs e)
		{
			lock (_lockObject)
			{
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