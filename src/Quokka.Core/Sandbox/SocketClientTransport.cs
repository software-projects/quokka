using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Quokka.Diagnostics;
using Quokka.Util;

namespace Quokka.Sandbox
{
	public class SocketClientTransport<TFrame, TFrameBuilder> : SocketTransport<TFrame>
		where TFrameBuilder : IFrameBuilder<TFrame>, new()
	{
		private Timer _timer;
		private bool _connectInProgress;

		public SocketClientTransport() : base(new TFrameBuilder())
		{

		}

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			
			if (disposing)
			{
				DisposeUtils.DisposeOf(ref _timer);
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
				if (Socket == null || !Socket.Connected && !_connectInProgress)
				{
					Socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
					Socket.BeginConnect(EndPoint, ConnectCallback, Socket);
					_connectInProgress = true;
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
