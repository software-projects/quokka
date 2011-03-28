using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Common.Logging;
using Quokka.Diagnostics;
using Quokka.Util;

namespace Quokka.Stomp.Transport
{
	public class SocketListener<TFrame, TFrameBuilder> : IListener<TFrame>
		where TFrameBuilder : IFrameBuilder<TFrame>, new()
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();
		private Socket _listenSocket;
		private readonly object _lockObject = new object();
		private readonly Queue<SocketTransport<TFrame>> _transports = new Queue<SocketTransport<TFrame>>();
		private Timer _timer;
		private bool _isDisposed;

		public event EventHandler ClientConnected;
		public event EventHandler<ExceptionEventArgs> ListenException;

		public IPEndPoint SpecifiedEndPoint { get; set; }
		public IPEndPoint ListenEndPoint { get; private set; }
		public int Backlog { get; set; }

		EndPoint IListener<TFrame>.ListenEndPoint
		{
			get { return ListenEndPoint; }
		}

		public SocketListener()
		{
			SpecifiedEndPoint = new IPEndPoint(IPAddress.Any, 0);
			Backlog = 127;
		}

		public void Dispose()
		{
			lock (_lockObject)
			{
				DisposeUtils.DisposeOf(ref _listenSocket);
				DisposeUtils.DisposeOf(ref _timer);
				_isDisposed = true;
			}
		}

		public void StartListening(EndPoint endPoint)
		{
			lock (_lockObject)
			{
				if (_timer == null)
				{
					_timer = new Timer(TimerCallback, this, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(10));
				}

				SpecifiedEndPoint = (IPEndPoint) endPoint;
				CreateListenSocketAndBeginAccept();
			}
		}

		public void StartListening()
		{
			StartListening(SpecifiedEndPoint);
		}

		private void TimerCallback(object obj)
		{
			try
			{
				lock (_lockObject)
				{
					if (_listenSocket == null)
					{
						CreateListenSocketAndBeginAccept();
					}
				}
			}
			catch (SocketException ex)
			{
				// TODO: handle port in use error
				HandleException(ex);
			}
			catch (Exception ex)
			{
				if (ex.IsCorruptedStateException())
				{
					throw;
				}
				HandleException(ex);
			}
		}

		private void CreateListenSocketAndBeginAccept()
		{
			_listenSocket = new Socket(SpecifiedEndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			_listenSocket.Bind(SpecifiedEndPoint);
			ListenEndPoint = (IPEndPoint)_listenSocket.LocalEndPoint;
			_listenSocket.Listen(Backlog);
			_listenSocket.BeginAccept(AcceptCallback, _listenSocket);
		}

		private void HandleException(Exception ex)
		{
			lock (_lockObject)
			{
				DisposeUtils.DisposeOf(ref _listenSocket);
			}

			OnListenException(new ExceptionEventArgs(ex));
		}

		protected virtual void OnListenException(ExceptionEventArgs e)
		{
			if (ListenException != null)
			{
				ListenException(this, e);
			}
		}

		public ITransport<TFrame> GetNextTransport()
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
			try
			{
				lock (_lockObject)
				{
					if (!_isDisposed)
					{
						Socket listenSocket = (Socket) ar.AsyncState;
						Socket handlerSocket = null;
						handlerSocket = listenSocket.EndAccept(ar);
						Log.Debug("Server accepted connection");
						if (!ReferenceEquals(listenSocket, _listenSocket))
						{
							// Here we have a connection, but we are no longer listening on that socket.
							// Disconnect the socket and throw it away.
							Log.Debug("Different listen socket, closing connection");
							handlerSocket.Shutdown(SocketShutdown.Both);
							handlerSocket.Disconnect(false);
							handlerSocket.Close();
							handlerSocket = null;
						}
						else
						{
							// Start waiting for the next connection
							listenSocket.BeginAccept(AcceptCallback, listenSocket);
						}

						if (handlerSocket != null)
						{
							var transport = new ServerTransport(handlerSocket, new TFrameBuilder());
							_transports.Enqueue(transport);
							OnClientConnected(EventArgs.Empty);
						}
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.IsCorruptedStateException())
				{
					throw;
				}
				HandleException(ex);
			}
		}

		protected void OnClientConnected(EventArgs e)
		{
			if (ClientConnected != null)
			{
				ClientConnected(this, e);
			}
		}

		private class ServerTransport : SocketTransport<TFrame>
		{
			public ServerTransport(Socket socket, IFrameBuilder<TFrame> frameBuilder) : base(frameBuilder)
			{
				Socket = socket;
				BeginReceive();
			}
		}
	}
}