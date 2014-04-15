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
using System.Diagnostics;
using System.Net;
using System.Threading;
using Castle.Core.Logging;
using NUnit.Framework;
using Quokka.Diagnostics;
using Quokka.Stomp.Transport;

namespace Quokka.Stomp
{
	[TestFixture]
	public class StompSocketTests
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();
		private StompListener _listener;
		private ITransport<StompFrame> _server;
		private StompClientTransport _clientTransport;

		private ManualResetEvent _finishedEvent;
		private Exception _ex;
		private string _message;
		private object _lockObject;
		private int _number;
		const int _maxNumber = 100;

		[SetUp]
		public void Setup()
		{
			_finishedEvent = new ManualResetEvent(false);
			_lockObject = new object();

			//Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter();
		}

		[TearDown]
		public void Teardown()
		{
			_finishedEvent = null;
			_lockObject = null;
			_ex = null;
			_message = null;
		}

		[Test]
		public void Test()
		{
			_listener = new StompListener();
			_listener.ClientConnected += ListenerClientConnected;
			_listener.StartListening();

			_clientTransport = new StompClientTransport
			          	{
			          		EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _listener.ListenEndPoint.Port)
			          	};
			_clientTransport.FrameReady += ClientFrameReady;
			_clientTransport.TransportException += ClientTransportException;
			_clientTransport.ConnectedChanged += ClientConnectedChanged;
			_clientTransport.Connect();

			if (Debugger.IsAttached)
			{
				_finishedEvent.WaitOne();
			}
			else
			{
				_finishedEvent.WaitOne(5000, true);
			}
			Assert.IsNull(_ex, "Exception encountered: " + _message + Environment.NewLine + _ex);
			Assert.AreEqual(_maxNumber, _number);
		}

		void ClientConnectedChanged(object sender, EventArgs e)
		{
			if (_clientTransport.Connected)
			{
				Log.Debug("Client: connected to server");
				const string text = "1";
				var frame = new StompFrame
				            	{
				            		Command = StompCommand.Message,
				            		BodyText = text,
				            	};
				_clientTransport.SendFrame(frame);
				Log.Debug("Client: sent message: " + text);
			}
			else
			{
				Log.Debug("Client: disconnected from server");
			}
		}

		private void ClientTransportException(object sender, ExceptionEventArgs e)
		{
			lock (_lockObject)
			{
				_ex = e.Exception;
				_message = "Client: transport exception";
				Log.Debug("Client: transport exception: " + _ex.Message);
			}

			_finishedEvent.Set();
		}

		private void ClientFrameReady(object sender, EventArgs e)
		{
			var frame = _clientTransport.GetNextFrame();
			if (frame == null)
			{
				return;
			}

			var text = frame.BodyText;

			Log.Debug("Client: received message: " + text);

			_number = int.Parse(text);
			if (_number == _maxNumber)
			{
				Log.Debug("Client: initiated shutdown");
				_clientTransport.Shutdown();
			}
			else
			{
				text = (_number + 1).ToString();
				frame.BodyText = text;
				_clientTransport.SendFrame(frame);
				Log.Debug("Client: sent message: " + text);
			}
		}

		private void ListenerClientConnected(object sender, EventArgs e)
		{
			Log.Debug("Listener: client connected");
			_server = _listener.GetNextTransport();
			_server.ConnectedChanged += ServerConnectionChanged;
			_server.FrameReady += ServerFrameReady;
			_server.TransportException += ServerTransportException;
			ProcessServerFrames(_server);
		}

		private void ServerConnectionChanged(object sender, EventArgs e)
		{
			if (!_server.Connected)
			{
				Log.Debug("Server: disconnected from client");
				_finishedEvent.Set();
			}
		}

		private static void ServerFrameReady(object sender, EventArgs e)
		{
			var server = (ITransport<StompFrame>) sender;
			ProcessServerFrames(server);
		}

		private static void ProcessServerFrames(ITransport<StompFrame> server)
		{
			for (; ; )
			{
				var frame = server.GetNextFrame();
				if (frame == null)
				{
					break;
				}

				var text = frame.BodyText;
				Log.Debug("Server: received message: " + text);
				server.SendFrame(frame);
			}
		}

		private void ServerTransportException(object sender, ExceptionEventArgs e)
		{
			lock (_lockObject)
			{
				_ex = e.Exception;
				_message = "Server: transport exception";
				Log.Debug("Server: transport exception: " + e.Exception.Message);
			}
			_finishedEvent.Set();
		}
	}
}