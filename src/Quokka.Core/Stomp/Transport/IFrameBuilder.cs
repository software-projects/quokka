using System;

namespace Quokka.Stomp.Transport
{
	///<summary>
	///	Used for assembling and serializing frames
	///</summary>
	///<typeparam name = "T"></typeparam>
	public interface IFrameBuilder<T>
	{
		///<summary>
		///	Convert the frame object into an array of bytes for transmission.
		///</summary>
		///<param name = "frame">
		///	The frame containing data to be transmitted.
		///</param>
		///<returns>
		///	An array segment of bytes to transmit.
		///</returns>
		ArraySegment<byte> ToArray(T frame);

		/// <summary>
		/// Allocate a read buffer for receiving bytes
		/// </summary>
		/// <returns></returns>
		ArraySegment<byte> GetReceiveBuffer();

		///<summary>
		///	Add bytes received to the builder
		///</summary>
		///<remarks>
		///	The data received may contain data for any number of frames, and may contain partial 
		///	data. Call <see cref = "IsFrameReady" /> and <see cref = "GetNextFrame" /> to retrieve the
		///	frame data.
		///</remarks>
		void ReceiveBytes(byte[] data, int offset, int length);

		///<summary>
		///	Is a frame ready to be retrieved by calling <see cref = "GetNextFrame" />.
		///</summary>
		bool IsFrameReady { get; }

		///<summary>
		///	Returns a frame that has been built from received data.
		///</summary>
		///<returns>
		///	A frame object, or <c>default(T)</c> if there is none.
		///</returns>
		T GetNextFrame();
	}
}