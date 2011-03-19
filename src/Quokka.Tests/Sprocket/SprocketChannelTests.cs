using System;
using System.Linq;
using System.Net;
using System.Threading;
using NUnit.Framework;
using Quokka.Collections;
using Quokka.Stomp;

// ReSharper disable InconsistentNaming

namespace Quokka.Sprocket
{
	[TestFixture]
	public class SprocketChannelTests
	{
		private StompServer _stompServer;
		private SprocketClient _client1;
		private SprocketClient _client2;
		private Uri _serverUrl;
		private DisposableCollection _disposables;
		private ManualResetEvent _waitEvent;

		[SetUp]
		public void SetUp()
		{
			_disposables = new DisposableCollection();
			_waitEvent = new ManualResetEvent(false);
			_disposables.Add(_waitEvent);
			_stompServer = new StompServer();
			_stompServer.ListenOn(new IPEndPoint(IPAddress.Loopback, 0));
			_disposables.Add(_stompServer);
			var ipEndPoint = (IPEndPoint) _stompServer.EndPoints.First();
			var url = "tcp://" + ipEndPoint.Address + ":" + ipEndPoint.Port;
			_serverUrl = new Uri(url);

			_client1 = new SprocketClient();
			_disposables.Add(_client1);
			_client1.Open(_serverUrl);

			_client2 = new SprocketClient();
			_disposables.Add(_client2);
			_client2.Open(_serverUrl);
		}

		[TearDown]
		public void TearDown()
		{
			if (_disposables != null)
			{
				_disposables.Dispose();
				_disposables = null;
			}
		}

		[Test]
		public void Times_out_when_there_is_no_subscriber()
		{
			bool timedOut = false;

			var channel = _client1.CreateChannel()
				.HandleTimeout(1, () =>
				                  	{
				                  		timedOut = true;
				                  		_waitEvent.Set();
				                  	})
				.AddTo(_disposables);

			channel.Send(new Request {Number = 999});

			_waitEvent.WaitOne(3000);

			Assert.IsTrue(timedOut);
		}

		[Test]
		public void Uses_synchronization_context_for_timeout()
		{
			bool timedOut = false;
			var sc = new TestSynchronizationContext();

			_client1.SynchronizationContext = sc;

			var channel = _client1.CreateChannel()
				.HandleTimeout(1, () =>
				                  	{
				                  		Assert.IsTrue(sc.IsInSend);
				                  		timedOut = true;
				                  		_waitEvent.Set();
				                  	})
				.AddTo(_disposables);

			channel.Send(new Request {Number = 999});

			_waitEvent.WaitOne(3000);

			Assert.IsTrue(timedOut);
		}

		[Test]
		public void Times_out_when_subscriber_takes_too_long()
		{
			bool timedOut = false;
			int result = 0;

			_client2.CreateSubscriber<Request>()
				.WithAction(request =>
				            	{
				            		Thread.Sleep(2000);
				            		_client2.Reply(new Response1 {Number = request.Number});
				            	})
				.AddTo(_disposables);

			var channel = _client1.CreateChannel()
				.HandleResponse<Response1>(response =>
				                           	{
				                           		result = response.Number;
				                           		_waitEvent.Set();
				                           	})
				.HandleTimeout(1, () =>
				                  	{
				                  		timedOut = true;
				                  		_waitEvent.Set();
				                  	})
				.AddTo(_disposables);

			channel.Send(new Request {Number = 999});

			_waitEvent.WaitOne(3000);

			Assert.IsTrue(timedOut);
			Assert.AreEqual(0, result);
		}

		[Test]
		public void Uses_synchronization_context_for_response()
		{
			int result = 0;
			bool wasInSend = false;

			var sc = new TestSynchronizationContext();

			_client1.SynchronizationContext = sc;

			_client2.CreateSubscriber<Request>()
				.WithAction(request => _client2.Reply(new Response1 {Number = request.Number}))
				.AddTo(_disposables);

			var channel = _client1.CreateChannel()
				.HandleResponse<Response1>(response =>
				                           	{
				                           		wasInSend = sc.IsInSend;
				                           		result = response.Number;
				                           		_waitEvent.Set();
				                           	})
				.AddTo(_disposables);

			channel.Send(new Request {Number = 42});

			_waitEvent.WaitOne();

			Assert.AreEqual(42, result);
			Assert.IsTrue(wasInSend);
		}

		[Test]
		public void Channel_with_one_request_and_two_responses()
		{
			int result = 0;
			int otherResult = 0;

			_client2.CreateSubscriber<Request>()
				.WithAction(HandleRequest)
				.AddTo(_disposables);

			var channel = _client1.CreateChannel()
				.HandleResponse<Response1>(response =>
				                           	{
				                           		result = response.Number;
				                           		_waitEvent.Set();
				                           	})
				.HandleResponse<Response2>(response => { otherResult = response.AnotherNumber; })
				.AddTo(_disposables);

			channel.Send(new Request {Number = 42});

			_waitEvent.WaitOne();

			Assert.AreEqual(84, result);
			Assert.AreEqual(42, otherResult);
		}

		private void HandleRequest(Request request)
		{
			_client2.Reply(new Response2 {AnotherNumber = request.Number});
			_client2.Reply(new Response1 {Number = request.Number*2});
		}

		public class Request
		{
			public int Number;
		}

		public class Response1
		{
			public int Number;
		}

		public class Response2
		{
			public int AnotherNumber;
		}

		private class TestSynchronizationContext : SynchronizationContext
		{
			public bool IsInSend { get; private set; }

			public override void Post(SendOrPostCallback d, object state)
			{
				Assert.Fail("Should not call Post method");
			}

			public override void Send(SendOrPostCallback d, object state)
			{
				Assert.IsFalse(IsInSend);
				IsInSend = true;
				try
				{
					d(state);
				}
				finally
				{
					IsInSend = false;
				}
			}
		}
	}
}