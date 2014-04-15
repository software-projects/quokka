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