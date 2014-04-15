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
	public interface IListener<TFrame> : IDisposable
	{
		event EventHandler ClientConnected;
		event EventHandler<ExceptionEventArgs> ListenException;

		EndPoint ListenEndPoint { get; }
		void StartListening();
		ITransport<TFrame> GetNextTransport();
	}
}