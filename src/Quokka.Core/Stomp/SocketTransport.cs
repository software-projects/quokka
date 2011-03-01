using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Quokka.Diagnostics;
using Quokka.Util;

namespace Quokka.Stomp
{
	/// <summary>
	/// Abstract base class for client and server socket transports.
	/// </summary>
	public abstract class SocketTransport : IStompTransport
	{
		protected readonly object LockObject = new object();
		protected StompFrameBuilder FrameBuilder;
		protected byte[] ReadBuffer;
		protected Socket Socket;
		private readonly Queue<ArraySegment<byte>> _segments = new Queue<ArraySegment<byte>>();
		private bool _sendInProgress;
		private bool _receiveInProgress;
		private bool _connected;

		public event EventHandler FrameReady;
		public event EventHandler ConnectedChanged;
		public event EventHandler<ExceptionEventArgs> TransportException;

		public bool Connected
		{
			get { return _connected; }
		}

		public StompFrame GetNextFrame()
		{
			lock (LockObject)
			{
				if (FrameBuilder != null)
				{
					return FrameBuilder.GetNextFrame();
				}
			}
			return null;
		}

		public void Send(StompFrame frame)
		{
			// convert the frame into an array of bytes to transmit
			byte[] data = frame.ToArray();

			lock (LockObject)
			{
				var segment = new ArraySegment<byte>(data);
				_segments.Enqueue(segment);
				if (!_sendInProgress)
				{
					ThreadPool.QueueUserWorkItem(StartSend);
				}
			}
		}

		public void BeginReceive()
		{
			lock (LockObject)
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

				if (ReadBuffer == null)
				{
					// Allocate a new read buffer if one has not been supplied
					ReadBuffer = new byte[4096];
				}

				try
				{
					Socket.BeginReceive(ReadBuffer, 0, ReadBuffer.Length, SocketFlags.None, ReceiveCallback, Socket);
					_receiveInProgress = true;
				}
				catch (Exception ex)
				{
					ThreadPool.QueueUserWorkItem(ExceptionCallback, ex);
				}
			}
		}

		private bool CheckConnected()
		{
			var connected = Socket == null ? false : Socket.Connected;
			if (connected != _connected)
			{
				_connected = connected;
				ThreadPool.QueueUserWorkItem(ConnectedCallback);
			}
			return connected;
		}

		private void ConnectedCallback(object state)
		{
			if (ConnectedChanged != null)
			{
				ConnectedChanged(this, EventArgs.Empty);
			}
		}


		protected virtual void OnTransportException(Exception ex)
		{
			
		}

		protected virtual void OnFrameReady()
		{
			if (FrameReady != null)
			{
				FrameReady(this, EventArgs.Empty);
			}
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

				if (_segments.Count == 0)
				{
					// Nothing to send
					return;
				}

				SendNextSegment();
			}
		}

		private void SendNextSegment()
		{
			var segment = _segments.Dequeue();
			var state = new SendState {Segment = segment, Socket = Socket};
			Socket.BeginSend(segment.Array, segment.Offset, segment.Count, SocketFlags.None, SendCallback, state);
			_sendInProgress = true;
		}

		private class SendState
		{
			public Socket Socket;
			public ArraySegment<byte> Segment;
		}

		private void SendCallback(IAsyncResult ar)
		{
			try
			{
				lock (LockObject)
				{
					_sendInProgress = false;
					var state = (SendState) ar.AsyncState;
					var byteCount = state.Socket.EndSend(ar);

					if (byteCount < state.Segment.Count)
					{
						// Not all bytes were sent in the segment, so it has to be put back on the queue
						var newSegment = new ArraySegment<byte>(state.Segment.Array,
						                                        state.Segment.Offset + byteCount,
						                                        state.Segment.Count - byteCount);
						state.Segment = newSegment;
						Socket.BeginSend(newSegment.Array, newSegment.Offset, newSegment.Count, SocketFlags.None, SendCallback, state);
						_sendInProgress = true;
					}
					else if (_segments.Count > 0)
					{
						SendNextSegment();
					}
				}
			}
			catch (Exception ex)
			{
				if (ex.IsCorruptedStateException())
				{
					throw;
				}

				// We are already on a worker thread here, so call the callback immediately.
				ExceptionCallback(ex);
			}
		}

		private void HandleException(Exception ex)
		{
			lock (LockObject)
			{
				OnTransportException(ex);
			}

			ThreadPool.QueueUserWorkItem(RaiseTransportException, ex);
		}

		private void ExceptionCallback(object state)
		{
			Exception ex = (Exception)state;

			lock (LockObject)
			{
				OnTransportException(ex);
			}

			if (TransportException != null)
			{
				TransportException(this, new ExceptionEventArgs(ex));
			}
		}

		private void ReceiveCallback(IAsyncResult ar)
		{
			var socket = ar.AsyncState as Socket;
			if (socket == null || !ReferenceEquals(socket, Socket))
			{
				// different socket, do nothing
				return;
			}

			bool raiseFrameReady = false;

			try
			{
				int byteCount = socket.EndReceive(ar);

				if (byteCount == 0)
				{
					socket.Shutdown(SocketShutdown.Send);
					socket.BeginDisconnect(false, DisconnectCallback, socket);
				}
				if (FrameBuilder.ReceiveBytes(ReadBuffer, 0, byteCount))
				{
					raiseFrameReady = true;
				}
				Socket.BeginReceive(ReadBuffer, 0, ReadBuffer.Length, SocketFlags.None, ReceiveCallback, Socket);
			}
			catch (Exception ex)
			{
				HandleException(ex);
			}
			finally
			{
				if (raiseFrameReady)
				{
					ThreadPool.QueueUserWorkItem(RaiseFrameReady);
				}
			}
		}

		private void DisconnectCallback(IAsyncResult ar)
		{
			try
			{
				var socket = (Socket) ar.AsyncState;
				socket.EndDisconnect(ar);
			}
			catch
			{
				// TODO
			}
		}

		private void RaiseFrameReady(object state)
		{
			if (FrameReady != null)
			{
				FrameReady(this, EventArgs.Empty);
			}
		}

		private void RaiseTransportException(object ex)
		{
			if (TransportException != null)
			{
				var exception = (Exception) ex;
				TransportException(this, new ExceptionEventArgs(exception));
			}
		}


	}


}