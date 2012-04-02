using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Castle.Core.Logging;
using Quokka.Diagnostics;
using Quokka.Util;

namespace Quokka.Stomp.Transport
{
	public class SocketClientTransport<TFrame, TFrameBuilder> : SocketTransport<TFrame>
		where TFrameBuilder : IFrameBuilder<TFrame>, new()
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private Timer _timer;
		private bool _connectInProgress;
		private bool _isDisposed;

		public SocketClientTransport() : base(new TFrameBuilder())
		{
		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);

			if (disposing)
			{
				lock (LockObject)
				{
					_isDisposed = true;
					DisposeUtils.DisposeOf(ref _timer);
				}
			}
		}

		public IPEndPoint EndPoint { get; set; }

		public void Connect()
		{
			lock (LockObject)
			{
				if (_timer == null)
				{
					_timer = new Timer(TimerCallback, this, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(10));
				}
			}
		}

		private void TimerCallback(object state)
		{
			lock (LockObject)
			{
				if (Socket == null || !Socket.Connected && !_connectInProgress && !_isDisposed)
				{
					Socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
					_connectInProgress = true;
					Socket.BeginConnect(EndPoint, ConnectCallback, Socket);
				}
			}
		}

		public void Connect(IPEndPoint endPoint)
		{
			EndPoint = endPoint;
			Connect();
		}

		private void ConnectCallback(IAsyncResult ar)
		{
			try
			{
				var socket = (Socket) ar.AsyncState;
				lock (LockObject)
				{
					if (socket != Socket)
					{
						// not the socket we are using
						return;
					}
					_connectInProgress = false;
					socket.EndConnect(ar);
					Log.Debug("Client socket connected");
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
			BeginReceive();
		}
	}
}