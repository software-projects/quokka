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

		/// <summary>
		/// The local endpoint for this transport.
		/// </summary>
		EndPoint LocalEndPoint { get; }

		/// <summary>
		/// The remote endpoint for this transport.
		/// </summary>
		EndPoint RemoteEndPoint { get; }
	}
}