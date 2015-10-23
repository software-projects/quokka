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
	/// <summary>
	/// 	Abstract base class for client and server socket transports.
	/// </summary>
	public abstract class SocketTransport<TFrame> : ITransport<TFrame>
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private readonly LockObject _lockObject = new LockObject();
		private readonly IFrameBuilder<TFrame> _frameBuilder;
		protected Socket Socket;
		private readonly Queue<TFrame> _pendingFrames = new Queue<TFrame>();
		private bool _sendInProgress;
		private bool _receiveInProgress;
		private bool _shutdownPending;
		private bool _connected;
		private bool _isDisposed;

		protected SocketTransport(IFrameBuilder<TFrame> frameBuilder)
		{
			_frameBuilder = Verify.ArgumentNotNull(frameBuilder, "frameBuilder");
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected IDisposable Lock()
		{
			return _lockObject.Lock();
		}

		protected void AfterUnlock(Action action)
		{
			_lockObject.AfterUnlock(action);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				using(Lock())
				{
					_isDisposed = true;

					if (Socket != null && !_shutdownPending)
					{
						if (Socket.Connected)
						{
							Socket.Shutdown(SocketShutdown.Both);
							// blocking disconnect
							Socket.Disconnect(false);
						}
					}
					if (!_shutdownPending)
					{
						DisposeUtils.DisposeOf(ref Socket);
					}
					_pendingFrames.Clear();
					_sendInProgress = false;
					_receiveInProgress = false;
					_connected = false;
				}
			}
		}

		#region ITransport<TFrame>

		public event EventHandler FrameReady;
		public event EventHandler ConnectedChanged;
		public event EventHandler<ExceptionEventArgs> TransportException;

		public bool Connected
		{
			get { return _connected; }
		}

		public EndPoint LocalEndPoint
		{
			get
			{
				if (Socket != null)
				{
					return Socket.LocalEndPoint;
				}
				return null;
			}
		}

		public EndPoint RemoteEndPoint
		{
			get
			{
				if (Socket != null)
				{
					return Socket.LocalEndPoint;
				}
				return null;
			}
		}


		public TFrame GetNextFrame()
		{
			using(Lock())
			{
				if (_frameBuilder != null && _frameBuilder.IsFrameReady)
				{
					var frame = _frameBuilder.GetNextFrame();
					return frame;
				}
			}
			return default(TFrame);
		}

		public void SendFrame(TFrame frame)
		{
			using(Lock())
			{
				if (_shutdownPending)
				{
					throw new InvalidOperationException("Shutdown pending");
				}

				_pendingFrames.Enqueue(frame);
				if (!_sendInProgress)
				{
					//Log.Debug("Queued StartSend");
					ThreadPool.QueueUserWorkItem(StartSend);
				}
			}
		}

		public void Shutdown()
		{
			using(Lock())
			{
				if (Socket != null)
				{

					if (Socket.Connected)
					{
						_shutdownPending = true;
						if (_pendingFrames.Count == 0 && !_sendInProgress)
						{
							BeginDisconnect();
						}
					}
					else
					{
						Socket.Close();
						DisposeUtils.DisposeOf(ref Socket);
					}
				}
			}
		}

		#endregion

		#region Protected overridable

		protected virtual void OnTransportException(ExceptionEventArgs e)
		{
			if (TransportException != null)
			{
				TransportException(this, e);
			}
		}

		protected virtual void OnConnectedChanged(EventArgs e)
		{
			if (ConnectedChanged != null)
			{
				ConnectedChanged(this, EventArgs.Empty);
			}
		}

		protected virtual void OnFrameReady(EventArgs e)
		{
			if (FrameReady != null)
			{
				FrameReady(this, e);
			}
		}

		protected virtual void HandleException(Exception ex)
		{
			using(Lock())
			{
				try
				{
					if (Socket != null)
					{
						Socket.Close();
						Socket = null;
					}
				}
				catch
				{
					Socket = null;
				}
				CheckConnected();
			}

			OnTransportException(new ExceptionEventArgs(ex));
		}

		#endregion

		#region Receiving

		private class ReceiveState
		{
			public Socket Socket;
			public ArraySegment<byte> Segment;
		}

		public void BeginReceive()
		{
			try
			{
				using(Lock())
				{
					BeginReceiveHelper();
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

		private void BeginReceiveHelper()
		{
			if (!CheckConnected())
			{
				// Cannot receive if the socket is not connected
				return;
			}

			if (_receiveInProgress)
			{
				// Only want one receive operation in progress at a time
				return;
			}

			var receiveBuffer = _frameBuilder.GetReceiveBuffer();

			var receiveState = new ReceiveState
			                   	{
			                   		Segment = receiveBuffer,
			                   		Socket = Socket,
			                   	};

			// Set _receiveInProgress prior to calling BeginReceive, as BeginReceive *might*
			// return synchronously, in which case the callback is processed on this thread.
			_receiveInProgress = true;
			Socket.BeginReceive(receiveBuffer.Array,
			                    receiveBuffer.Offset,
			                    receiveBuffer.Count,
			                    SocketFlags.None,
			                    ReceiveCallbackMightBeSynchronous,
			                    receiveState);
		}

		private void ReceiveCallbackMightBeSynchronous(IAsyncResult ar)
		{
			if (ar.CompletedSynchronously)
			{
				Log.Debug("BeginReceive was completed synchronously");
				ThreadPool.QueueUserWorkItem(delegate { ReceiveCallbackOnWorkerThread(ar); });
			}
			else
			{
				ReceiveCallbackOnWorkerThread(ar);
			}
		}

		private void ReceiveCallbackOnWorkerThread(IAsyncResult ar)
		{
			try
			{
				using (Lock())
				{
					if (_isDisposed)
					{
						return;
					}

					var state = (ReceiveState)ar.AsyncState;
					if (state.Socket != Socket || Socket == null)
					{
						// Different socket, or we no longer have a socket, so do not complete the read
						return;
					}

					_receiveInProgress = false;
					int byteCount = Socket.EndReceive(ar);

					if (byteCount == 0)
					{
						// The other side is shutting down
						BeginDisconnect();
					}
					else
					{
						_frameBuilder.ReceiveBytes(state.Segment.Array, state.Segment.Offset, byteCount);
						if (_frameBuilder.IsFrameReady)
						{
							AfterUnlock(() => OnFrameReady(EventArgs.Empty));
						}
						BeginReceiveHelper();
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

		#endregion

		#region Sending

		private class SendState
		{
			public Socket Socket;
			public ArraySegment<byte> Segment;
		}

		private void StartSend(object obj)
		{
			//Log.Debug("-> StartSend");
			using (Lock())
			{
				if (!CheckConnected())
				{
					// cannot send if not connected
					return;
				}

				if (_sendInProgress)
				{
					// Another thread has already started sending
					return;
				}

				if (_pendingFrames.Count == 0)
				{
					// Nothing to send
					return;
				}

				SendNextFrame();
			}
			//Log.Debug("<- StartSend");
		}

		private void SendNextFrame()
		{
			// Get the front frame from the queue, but do not dequeue it.
			// If we disconnect part-way through sending the frame, we want
			// the chance to retransmit it.
			var frame = _pendingFrames.Peek();
			var segment = _frameBuilder.ToArray(frame);
			SendSegment(segment);
		}

		private void SendSegment(ArraySegment<byte> segment)
		{
			var state = new SendState {Segment = segment, Socket = Socket};

			// It is important to set this to true *prior* to calling BeginSend. The reason
			// is that BeginSend *might* finish synchronously.
			_sendInProgress = true;
			Socket.BeginSend(segment.Array, segment.Offset, segment.Count, SocketFlags.None, SendCallback, state);
		}

		private void SendCallback(IAsyncResult ar)
		{
			//Log.Debug("-> SendCallback");
			if (ar.CompletedSynchronously)
			{
				Log.Debug("BeginSend was completed synchronously");
			}
			var state = (SendState) ar.AsyncState;
			if (state.Socket != Socket)
			{
				// callback from previous connection -- ignore
				//Log.Debug("<- SendCallback (ignoring)");
				return;
			}

			try
			{
				using (Lock())
				{
					if (_isDisposed)
					{
						return;
					}

					_sendInProgress = false;
					var byteCount = state.Socket.EndSend(ar);

					if (byteCount < state.Segment.Count)
					{
						Log.DebugFormat("Only {0} out of {1} bytes transmitted", byteCount, state.Segment.Count);
						// Not all bytes were sent in the segment, so the remaining data has to be sent
						var newSegment = new ArraySegment<byte>(state.Segment.Array,
						                                        state.Segment.Offset + byteCount,
						                                        state.Segment.Count - byteCount);
						SendSegment(newSegment);
					}
					else
					{
						// the entire segment was sent, so remove the first frame from the queue
						_pendingFrames.Dequeue();
						if (_pendingFrames.Count > 0)
						{
							SendNextFrame();
						}
						else if (_shutdownPending)
						{
							BeginDisconnect();
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
			//Log.Debug("<- SendCallback");
		}

		#endregion

		#region Connecting and Disconnectiong

		private bool CheckConnected()
		{
			var connected = Socket != null && Socket.Connected;
			if (connected != _connected)
			{
				_connected = connected;

				if (_connected)
				{
					BeginReceive();
				}
				else
				{
					_sendInProgress = false;
					_receiveInProgress = false;
					_shutdownPending = false;
				}
				ThreadPool.QueueUserWorkItem(ConnectedCallback);
			}
			return connected;
		}

		private void ConnectedCallback(object state)
		{
			OnConnectedChanged(EventArgs.Empty);
		}

		private void BeginDisconnect()
		{
			// The other side is shutting down
			Socket.Shutdown(SocketShutdown.Both);
			Socket.BeginDisconnect(false, DisconnectCallback, Socket);
		}

		private void DisconnectCallback(IAsyncResult ar)
		{
			if (ar.CompletedSynchronously)
			{
				Log.Debug("BeginDisconnect was completed synchronously");
			}

			var socket = (Socket) ar.AsyncState;

			try
			{
				using (Lock())
				{
					if (!_isDisposed)
					{
						try
						{
							// The socket may well have been disposed by the time this
							// operation completes.
							socket.EndDisconnect(ar);
							socket.Close();
						}
						catch (ObjectDisposedException)
						{
							Log.Debug("Socket has already been disposed");
						}

						if (Socket == socket)
						{
							Socket = null;
						}
						CheckConnected();
					}
					DisposeUtils.DisposeOf(ref Socket);
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

		#endregion
	}
}