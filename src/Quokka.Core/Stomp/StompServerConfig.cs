﻿#region License

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
using Quokka.Stomp.Server.Messages;

namespace Quokka.Stomp
{
	public class StompServerConfig
	{
		/// <summary>
		/// 	Time between cleanups.
		/// </summary>
		/// <remarks>
		/// 	The server periodically runs a cleanup process where
		/// 	unused resources associated with sessions and message
		/// 	queues are cleaned up. This is the time interval between
		/// 	cleanups.
		/// </remarks>
		public TimeSpan CleanupPeriod { get; set; }

		/// <summary>
		///		Timeout for unused sessions
		/// </summary>
		/// <remarks>
		///		If a connection is abruptly lost with a client, the session information
		///		is kept in case the client is able to reconnect in a short period of time.
		///		If the client has not reconnected in the time specified here, the session
		///		information is cleaned up.
		/// </remarks>
		public TimeSpan UnusedSessionTimeout { get; set; }

		/// <summary>
		///		Time between sending server status messages.
		/// </summary>
		/// <remarks>
		///		When a client has subscribed to the <see cref="ServerStatusMessage"/> message
		///		queue, it will receive regular server status messages. This parameter defines
		///		how often the server will send these messages.
		/// </remarks>
		public TimeSpan ServerStatusPeriod { get; set; }

		/// <summary>
		///		The time that the server will wait between accepting a connection and receiving
		///		a CONNECT frame or a STOMP frame.
		/// </summary>
		public TimeSpan ConnectFrameTimeout { get; set; }

		public StompServerConfig()
		{
			CleanupPeriod = TimeSpan.FromMinutes(1);
			UnusedSessionTimeout = TimeSpan.FromMinutes(5);
			ServerStatusPeriod = TimeSpan.FromSeconds(5);
			ConnectFrameTimeout = TimeSpan.FromSeconds(30);
		}
	}
}