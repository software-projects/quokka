using System;

namespace Quokka.Stomp.Transport
{
	public interface ITransport<TFrame> : IDisposable
	{
		///<summary>
		///	Raised when one or more frames have been received from
		///	the other station. The frames can be read by repeated
		///	calls to <see cref = "GetNextFrame" />.
		///</summary>
		event EventHandler FrameReady;

		/// <summary>
		/// 	Raised whenever the value of <see cref = "Connected" /> changes.
		/// </summary>
		event EventHandler ConnectedChanged;

		/// <summary>
		/// 	Raised when an exception occurs during communications with
		/// 	the other station.
		/// </summary>
		event EventHandler<ExceptionEventArgs> TransportException;

		/// <summary>
		/// 	Is the transport layer connected to the other station.
		/// </summary>
		bool Connected { get; }

		/// <summary>
		/// 	Receive the next frame received from the other station.
		/// </summary>
		/// <returns>
		/// 	Returns a <typeparamref name="TFrame"/>object, or <c>null</c> if
		/// 	no frames are available.
		/// </returns>
		TFrame GetNextFrame();

		/// <summary>
		/// 	Queue a <typeparamref name = "TFrame" /> to be transmitted to the other station as soon as possible.
		/// </summary>
		/// <param name = "frame">
		/// 	The frame to be transmitted to the other station.
		/// </param>
		/// <remarks>
		/// 	The calling program should not modify the contents of <see cref = "frame" /> after calling this method.
		/// </remarks>
		void SendFrame(TFrame frame);

		/// <summary>
		///		Closes the connection after ensuring that all pending frames have been transmitted.
		/// </summary>
		void Shutdown();
	}
}