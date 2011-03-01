using System;

namespace Quokka.Stomp
{
	public interface IStompTransport
	{
		/// <summary>
		/// 	Is the transport layer connected to the other station.
		/// </summary>
		bool Connected { get; }

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
		/// 	Start communicating with the other station. (Not sure we want this here).
		/// </summary>
		//void Start();

		/// <summary>
		/// 	Receive the next frame received from the other station.
		/// </summary>
		/// <returns>
		/// 	Returns a <see cref = "StompFrame" /> object, or <c>null</c> if
		/// 	no frames are available.
		/// </returns>
		StompFrame GetNextFrame();

		/// <summary>
		/// 	Queue a <see cref = "StompFrame" /> to be transmitted to the other station as soon as possible.
		/// </summary>
		/// <param name = "frame">
		/// 	The frame to be transmitted to the other station.
		/// </param>
		/// <remarks>
		/// 	The calling program should not modify the contents of <see cref = "frame" /> after calling this method.
		/// </remarks>
		void Send(StompFrame frame);
	}
}