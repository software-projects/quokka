#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Castle.Core.Logging;
using Quokka.Diagnostics;
using Quokka.Stomp.Internal;
using Quokka.Util;

namespace Quokka.Stomp.Transport
{
	public class SocketListener<TFrame, TFrameBuilder> : IListener<TFrame>
		where TFrameBuilder : IFrameBuilder<TFrame>, new()
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private Socket _listenSocket;
		private readonly LockObject _lockObject = new LockObject();
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
			using (_lockObject.Lock())
			{
				DisposeUtils.DisposeOf(ref _listenSocket);
				DisposeUtils.DisposeOf(ref _timer);
				_isDisposed = true;
				ClientConnected = null;
				ListenException = null;
			}
		}

		public void StartListening(EndPoint endPoint)
		{
			using (_lockObject.Lock())
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
				using (_lockObject.Lock())
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
			using (_lockObject.Lock())
			{
				DisposeUtils.DisposeOf(ref _listenSocket);
				_lockObject.AfterUnlock(() => OnListenException(new ExceptionEventArgs(ex)));
			}
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
			using (_lockObject.Lock())
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
			if (ar.CompletedSynchronously)
			{
				ThreadPool.QueueUserWorkItem(delegate { AcceptCallbackOnWorkerThread(ar); }, null);
			}
			else
			{
				AcceptCallbackOnWorkerThread(ar);
			}
		}

		private void AcceptCallbackOnWorkerThread(IAsyncResult ar)
		{
			try
			{
				using (_lockObject.Lock())
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
							_lockObject.AfterUnlock(() => OnClientConnected(EventArgs.Empty));
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