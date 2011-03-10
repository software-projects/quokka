using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Quokka.Stomp
{
	[TestFixture]
	public class StompServerTests
	{
		[SetUp]
		public void SetUp()
		{
			Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter();
		}

		[Test]
		public void Provides_end_point_actually_being_listened_on()
		{
			// provide the endpoint which is 'listen on any IP address and any port'
			var endPoint = new IPEndPoint(IPAddress.Any, 0);

			StompServer server = new StompServer();
			server.ListenOn(endPoint);

			// Want to know the actual port being listened on
			var listenEndPoint = (IPEndPoint) server.EndPoints.First();
			Assert.IsTrue(listenEndPoint.Port > 0, "Should be endpoint actually being listened to");

			server.Dispose();
		}

		[Test]
		public void Server_can_listen_on_multiple_endpoints()
		{
			// provide the endpoint which is 'listen on any IP address and any port'
			var endPoint = new IPEndPoint(IPAddress.Any, 0);

			StompServer server = new StompServer();
			server.ListenOn(endPoint);
			server.ListenOn(endPoint);

			Assert.AreEqual(2, server.EndPoints.Count);

			// Want to know the actual port being listened on
			var listenEndPoint1 = (IPEndPoint)server.EndPoints.First();
			var listenEndPoint2 = (IPEndPoint) server.EndPoints.Last();
			Assert.AreNotSame(listenEndPoint1, listenEndPoint2);
			Assert.IsTrue(listenEndPoint1.Port > 0, "Should be endpoint actually being listened to");
			Assert.IsTrue(listenEndPoint2.Port > 0, "Should be endpoint actually being listened to");
			Assert.AreNotEqual(listenEndPoint1.Port, listenEndPoint2.Port);

			server.Dispose();
		}

		[Test]
		public void Simple_message_exchange()
		{
			StompServer server = new StompServer();
			var endPoint = new IPEndPoint(IPAddress.Any, 0);
			server.ListenOn(endPoint);

			var serverPort = ((IPEndPoint) server.EndPoints.First()).Port;
			var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);

			StompClient client1 = new StompClient();
			client1.ConnectTo(serverEndPoint);

			StompClient client2 = new StompClient();
			client2.ConnectTo(serverEndPoint);

			const string queueName = "/queue";

			string receivedText = null;
			int receivedMessageCount = 0;
			var waitHandle = new ManualResetEvent(false);

			var subscription = client2.CreateSubscription(queueName);
			subscription.MessageArrived += delegate(object sender, StompMessageEventArgs e)
			                               	{
			                               		receivedText = e.Message.BodyText;
			                               		Interlocked.Increment(ref receivedMessageCount);
			                               		waitHandle.Set();
			                               	};
			subscription.Subscribe();

			client1.SendTextMessage(queueName, "This is a simple message");

			if (Debugger.IsAttached)
			{
				waitHandle.WaitOne();
			}
			else
			{
				waitHandle.WaitOne(2000);
			}

			Assert.AreEqual(receivedText, "This is a simple message");
		}

	}
}
