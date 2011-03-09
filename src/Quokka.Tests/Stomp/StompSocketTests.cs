using System;
using System.Net;
using System.Threading;
using NUnit.Framework;
using Quokka.Sandbox;

namespace Quokka.Stomp
{
	[TestFixture]
	public class StompSocketTests
	{
		private StompListener _listener;
		private ITransport<StompFrame> _server;
		private StompClient _client;

		private ManualResetEvent _finishedEvent;
		private Exception _ex;
		private string _message;
		private object _lockObject;
		private int _number;

		[SetUp]
		public void Setup()
		{
			_finishedEvent = new ManualResetEvent(false);
			_lockObject = new object();
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

			_client = new StompClient
			          	{
			          		EndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), _listener.ListenEndPoint.Port)
			          	};
			_client.FrameReady += ClientFrameReady;
			_client.TransportException += ClientTransportException;
			_client.ConnectedChanged += ClientConnectedChanged;
			_client.Connect();

			Assert.IsTrue(_finishedEvent.WaitOne(5000, true));
			Assert.IsNull(_ex, "Exception encountered: " + _message + Environment.NewLine + _ex);
			Assert.AreEqual(10, _number);
		}

		void ClientConnectedChanged(object sender, EventArgs e)
		{
			if (_client.Connected)
			{
				Console.WriteLine("Client: connected to server");
				const string text = "1";
				var frame = new StompFrame
				            	{
				            		Command = StompCommand.Message,
				            		BodyText = text,
				            	};
				_client.SendFrame(frame);
				Console.WriteLine("Client: sent message: " + text);
			}
			else
			{
				Console.WriteLine("Client: disconnected from server");
			}
		}

		private void ClientTransportException(object sender, ExceptionEventArgs e)
		{
			lock (_lockObject)
			{
				_ex = e.Exception;
				_message = "Client: transport exception";
				Console.WriteLine("Client: transport exception: " + _ex.Message);
			}

			_finishedEvent.Set();
		}

		private void ClientFrameReady(object sender, EventArgs e)
		{
			var frame = _client.GetNextFrame();
			if (frame == null)
			{
				return;
			}

			var text = frame.BodyText;

			Console.WriteLine("Client: received message: " + text);

			_number = int.Parse(text);
			if (_number == 10)
			{
				Console.WriteLine("Client: initiated shutdown");
				_client.Shutdown();
			}
			else
			{
				text = (_number + 1).ToString();
				frame.BodyText = text;
				_client.SendFrame(frame);
				Console.WriteLine("Client: sent message: " + text);
			}
		}

		private void ListenerClientConnected(object sender, EventArgs e)
		{
			Console.WriteLine("Listener: client connected");
			_server = _listener.GetNextTransport();
			_server.ConnectedChanged += ServerConnectionChanged;
			_server.FrameReady += ServerFrameReady;
			_server.TransportException += ServerTransportException;
		}

		private void ServerConnectionChanged(object sender, EventArgs e)
		{
			if (!_server.Connected)
			{
				Console.WriteLine("Server: disconnected from client");
				_finishedEvent.Set();
			}
		}

		private static void ServerFrameReady(object sender, EventArgs e)
		{
			var server = (ITransport<StompFrame>) sender;
			var frame = server.GetNextFrame();

			var text = frame.BodyText;

			Console.WriteLine("Server: received message: " + text);
			server.SendFrame(frame);
		}

		private void ServerTransportException(object sender, ExceptionEventArgs e)
		{
			lock (_lockObject)
			{
				_ex = e.Exception;
				_message = "Server: transport exception";
				Console.WriteLine("Server: transport exception: " + e.Exception.Message);
			}
			_finishedEvent.Set();
		}
	}
}