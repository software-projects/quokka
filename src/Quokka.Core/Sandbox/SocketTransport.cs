using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using Quokka.Diagnostics;
using Quokka.Util;

namespace Quokka.Sandbox
{
	/// <summary>
	/// 	Abstract base class for client and server socket transports.
	/// </summary>
	public abstract class SocketTransport<TFrame> : ITransport<TFrame>
	{
		protected readonly object LockObject = new object();
		private readonly IFrameBuilder<TFrame> _frameBuilder;
		protected Socket Socket;
		private readonly Queue<TFrame> _pendingFrames = new Queue<TFrame>();
		private bool _sendInProgress;
		private bool _receiveInProgress;
		private bool _shutdownPending;
		private bool _connected;

		protected SocketTransport(IFrameBuilder<TFrame> frameBuilder)
		{
			_frameBuilder = Verify.ArgumentNotNull(frameBuilder, "frameBuilder");
		}

		public void Dispose()
		{
			Dispose(true);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
				DisposeUtils.DisposeOf(ref Socket);
				_pendingFrames.Clear();
				_sendInProgress = false;
				_receiveInProgress = false;
				_shutdownPending = false;
				_connected = false;
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

		public TFrame GetNextFrame()
		{
			lock (LockObject)
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
			lock (LockObject)
			{
				if (_shutdownPending)
				{
					throw new InvalidOperationException("Shutdown pending");
				}

				_pendingFrames.Enqueue(frame);
				if (!_sendInProgress)
				{
					ThreadPool.QueueUserWorkItem(StartSend);
				}
			}
		}

		public void Shutdown()
		{
			lock (LockObject)
			{
				_shutdownPending = true;
				if (_pendingFrames.Count == 0 && !_sendInProgress)
				{
					BeginDisconnect();
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
			lock (LockObject)
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
				lock (LockObject)
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

			Socket.BeginReceive(receiveBuffer.Array, receiveBuffer.Offset, receiveBuffer.Count, SocketFlags.None, ReceiveCallback,
			                    receiveState);
			_receiveInProgress = true;
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			var state = (ReceiveState) ar.AsyncState;
			if (state.Socket != Socket)
			{
				// different socket, do nothing
				return;
			}

			bool raiseFrameReady = false;

			try
			{
				lock (LockObject)
				{
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
							raiseFrameReady = true;
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
			finally
			{
				if (raiseFrameReady)
				{
					OnFrameReady(EventArgs.Empty);
				}
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
			lock (LockObject)
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
		}

		private void SendNextFrame()
		{
			// Get the front frame from the queue, but do not dequeue it.
			// If we disconnect part-way through sending the frame, we want
			// the chance to retransmit it.
			var frame = _pendingFrames.Peek();
			var data = _frameBuilder.ToArray(frame);
			var segment = new ArraySegment<byte>(data);
			SendSegment(segment);
		}

		private void SendSegment(ArraySegment<byte> segment)
		{
			var state = new SendState {Segment = segment, Socket = Socket};
			Socket.BeginSend(segment.Array, segment.Offset, segment.Count, SocketFlags.None, SendCallback, state);
			_sendInProgress = true;
		}

		private void SendCallback(IAsyncResult ar)
		{
			var state = (SendState) ar.AsyncState;
			if (state.Socket != Socket)
			{
				// callback from previous connection -- ignore
				return;
			}

			try
			{
				lock (LockObject)
				{
					_sendInProgress = false;
					var byteCount = state.Socket.EndSend(ar);

					if (byteCount < state.Segment.Count)
					{
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
		}

		#endregion

		#region Connecting and Disconnectiong

		private bool CheckConnected()
		{
			var connected = Socket == null ? false : Socket.Connected;
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
			var socket = (Socket) ar.AsyncState;

			try
			{
				lock (LockObject)
				{
					socket.EndDisconnect(ar);
					socket.Close();

					if (Socket == socket)
					{
						Socket = null;
					}
					CheckConnected();
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