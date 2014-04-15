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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Castle.Core.Logging;
using NUnit.Framework;
using Quokka.Diagnostics;
using Quokka.Stomp.Internal;

// ReSharper disable InconsistentNaming

namespace Quokka.Stomp
{
	[TestFixture]
	public class StompServerTests
	{
		[SetUp]
		public void SetUp()
		{
			//Common.Logging.LogManager.Adapter = new Common.Logging.Simple.ConsoleOutLoggerFactoryAdapter();
			LoggerFactory.SetLoggerFactory(new ConsoleFactory());
		}

		public void TearDown()
		{
			LoggerFactory.SetLoggerFactory(null);
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
			var subscribedWaitHandle = new ManualResetEvent(false);

			var subscription = client2.CreateSubscription(queueName);
			subscription.StateChanged += delegate
			                             	{
												if (subscription.State == StompSubscriptionState.Subscribed)
												{
													subscribedWaitHandle.Set();
												}
			                             	};
			subscription.MessageArrived += delegate(object sender, StompMessageEventArgs e)
			                               	{
			                               		receivedText = e.Message.BodyText;
			                               		Interlocked.Increment(ref receivedMessageCount);
			                               		waitHandle.Set();
			                               	};
			subscription.Subscribe();

			WaitOnHandle(subscribedWaitHandle, 2000, "Timed out waiting for subscription");
			client1.SendTextMessage(queueName, "This is a simple message");
			WaitOnHandle(waitHandle, 2000, "Timed out waiting for message");
			Assert.AreEqual("This is a simple message", receivedText);
		}

		[Test]
		public void Times_out_waiting_for_connect_frame()
		{
			const int timeout = 500;
			StompServer server = new StompServer();
			server.Config.ConnectFrameTimeout = TimeSpan.FromMilliseconds(timeout);
			var endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 0);
			server.ListenOn(endPoint);
			var socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
			socket.Connect(server.EndPoints.First());
			Assert.IsTrue(socket.Connected);
			Thread.Sleep(timeout * 2);

			// receive bytes from server -- this code assumes that we get the entire
			// frame in one read
			var frameBuilder = new StompFrameBuilder();
			do
			{
				var buffer = frameBuilder.GetReceiveBuffer();
				int byteCount = socket.Receive(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None);
				frameBuilder.ReceiveBytes(buffer.Array, buffer.Offset, byteCount);
			} while (!frameBuilder.IsFrameReady);
			var frame = frameBuilder.GetNextFrame();
			Assert.IsNotNull(frame);
			Assert.AreEqual(StompCommand.Error, frame.Command);

			// Perform another read, and we should get zero bytes, indicating that the other
			// side has shutdown the connection.
			{
				var buffer = frameBuilder.GetReceiveBuffer();
				int byteCount = socket.Receive(buffer.Array, buffer.Offset, buffer.Count, SocketFlags.None);
				Assert.AreEqual(0, byteCount);
			}
		}

		[Test]
		public void Publish_to_multiple_clients()
		{
			StompServer server = new StompServer();
			var endPoint = new IPEndPoint(IPAddress.Any, 0);
			server.ListenOn(endPoint);

			var serverPort = ((IPEndPoint)server.EndPoints.First()).Port;
			var serverEndPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), serverPort);

			StompClient client1 = new StompClient();
			client1.ConnectTo(serverEndPoint);

			StompClient client2 = new StompClient();
			client2.ConnectTo(serverEndPoint);

			StompClient client3 = new StompClient();
			client3.ConnectTo(serverEndPoint);

			const string queueName = "myqueue";
			const string publishQueueName = QueueName.PublishPrefix + queueName;

			string received1Text = null;
			string received2Text = null;
			int receivedMessageCount = 0;
			var waitHandle = new ManualResetEvent(false);
			var subscribedHandle1 = new ManualResetEvent(false);
			var subscribedHandle2 = new ManualResetEvent(false);

			var subscription1 = client1.CreateSubscription(queueName);
			subscription1.MessageArrived += delegate(object sender, StompMessageEventArgs e)
			                                	{
			                                		received1Text = e.Message.BodyText;
			                                		if (Interlocked.Increment(ref receivedMessageCount) == 2)
			                                		{
			                                			waitHandle.Set();
			                                		}
			                                	};
			subscription1.StateChanged += delegate
			                              	{
												if (subscription1.State == StompSubscriptionState.Subscribed)
												{
													subscribedHandle1.Set();
												}
			                              	};
			subscription1.Subscribe();

			var subscription2 = client2.CreateSubscription(queueName);
			subscription2.MessageArrived += delegate(object sender, StompMessageEventArgs e)
			                                	{
			                                		received2Text = e.Message.BodyText;
			                                		if (Interlocked.Increment(ref receivedMessageCount) == 2)
			                                		{
			                                			waitHandle.Set();
			                                		}
			                                	};
			subscription2.StateChanged += delegate
			                              	{
			                              		if (subscription2.State == StompSubscriptionState.Subscribed)
			                              		{
			                              			subscribedHandle2.Set();
			                              		}
			                              	};

			subscription2.Subscribe();

			const string messageText = "This is a published message";

			// wait untili the subscriptions have been sent to the server and confirmed
			WaitHandle.WaitAll(new[] {subscribedHandle1, subscribedHandle2});


			client1.SendTextMessage(publishQueueName, messageText);

			if (Debugger.IsAttached)
			{
				waitHandle.WaitOne();
			}
			else
			{
				waitHandle.WaitOne(2000);
			}

			Assert.AreEqual(messageText, received1Text, "Unexpected value for received1Text");
			Assert.AreEqual(messageText, received2Text, "Unexpected value for received2Text");
		}

		private static void WaitOnHandle(WaitHandle waitHandle, int milliseconds, string timeoutMessage)
		{
			if (Debugger.IsAttached)
			{
				waitHandle.WaitOne();
			}
			else
			{
				Assert.IsTrue(waitHandle.WaitOne(milliseconds), timeoutMessage);
			}
		}
	}
}
