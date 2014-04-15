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
		// ReSharper disable StaticFieldInGenericType
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		// ReSharper restore StaticFieldInGenericType
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
				using (Lock())
				{
					_isDisposed = true;
					DisposeUtils.DisposeOf(ref _timer);
				}
			}
		}

		public IPEndPoint EndPoint { get; set; }

		public void Connect()
		{
			using (Lock())
			{
				if (_timer == null)
				{
					_timer = new Timer(TimerCallback, this, TimeSpan.FromMilliseconds(0), TimeSpan.FromSeconds(10));
				}
			}
		}

		private void TimerCallback(object state)
		{
			using (Lock())
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
				using (Lock())
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