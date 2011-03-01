using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Quokka.Diagnostics;
using Quokka.Util;

namespace Quokka.Stomp
{
	public class SocketListener : IStompListener
	{
		private Socket _listenSocket;
		private readonly object _lockObject = new object();
		private readonly Queue<SocketTransport> _transports = new Queue<SocketTransport>();
		private Timer _timer;

		public event EventHandler ClientConnected;
		public event EventHandler<ExceptionEventArgs> ListenException;

		public IPEndPoint EndPoint { get; set; }
		public int Backlog { get; set; }

		public SocketListener()
		{
			EndPoint = new IPEndPoint(IPAddress.Any, 6720);
			Backlog = 127;
		}

		public void Dispose()
		{
			lock (_lockObject)
			{
				DisposeUtils.DisposeOf(ref _listenSocket);
				DisposeUtils.DisposeOf(ref _timer);
			}
		}

		public void StartListening()
		{
			lock (_lockObject)
			{
				if (_timer == null)
				{
					_timer = new Timer(TimerCallback, this, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));
				}
				DisposeUtils.DisposeOf(ref _listenSocket);
				DoStartListening();
			}
		}

		private void TimerCallback(object obj)
		{
			lock (_lockObject)
			{
				if (_listenSocket == null)
				{
					DoStartListening();
				}
			}
		}

		private void DoStartListening()
		{
			_listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			_listenSocket.Bind(EndPoint);
			_listenSocket.Listen(Backlog);
			_listenSocket.BeginAccept(AcceptCallback, _listenSocket);
		}

		public IStompTransport GetNextTransport()
		{
			lock (_lockObject)
			{
				if (_transports.Count > 0)
				{
					return _transports.Dequeue();
				}
			}
			return null;
		}

		private void AcceptCallback(IAsyncResult ar)
		{
			lock (_lockObject)
			{
				Socket handlerSocket = null;
				Socket listenSocket = (Socket)ar.AsyncState;
				try
				{
					handlerSocket = listenSocket.EndAccept(ar);
					if (!ReferenceEquals(listenSocket, _listenSocket))
					{
						// Here we have a connection, but we are no longer listening on that socket.
						// Disconnect the socket and throw it away.
						handlerSocket.Shutdown(SocketShutdown.Both);
						// TODO: This is inline, but we are on a worker thread, so it is
						// not a big problem?
						handlerSocket.Disconnect(false);
						handlerSocket.Close();
						handlerSocket = null;
					}
				}
				catch (Exception ex)
				{
					ThreadPool.QueueUserWorkItem(state => RaiseListenException(ex));
				}

				if (handlerSocket != null)
				{
					var transport = new ServerTransport(handlerSocket);
				}
			}

		}

		private void RaiseListenException(Exception ex)
		{
			if (ListenException != null)
			{
				ListenException(this, new ExceptionEventArgs(ex));
			}
		}

		private class ServerTransport : SocketTransport
		{
			public ServerTransport(Socket socket)
			{
				Socket = socket;
				BeginReceive();
			}
		}
	}
}