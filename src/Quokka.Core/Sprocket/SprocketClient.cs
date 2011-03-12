﻿using System;
using System.Net;
using System.Threading;
using Quokka.Stomp;
using Quokka.Stomp.Internal;

namespace Quokka.Sprocket
{
	public partial class SprocketClient : ISprocket, IDisposable
	{
		private StompClient _client;
		private bool _connected;
		private Uri _uri;

		public StompClient Client
		{
			get { return _client; }
		}

		public event EventHandler ConnectedChanged;

		public bool Connected
		{
			get { return _connected; }
			private set
			{
				if (_connected != value)
				{
					_connected = value;
					RaiseConnectedChanged();
				}
			}
		}

		public Uri ServerUrl
		{
			get { return _uri; }
		}

		public void Open(Uri uri)
		{
			Close();
			if (uri.Scheme != "tcp")
			{
				throw new ArgumentException("The only URI scheme supported is tcp");
			}
			var host = uri.Host;
			var port = uri.Port;
			IPAddress[] ipAddresses = Dns.GetHostAddresses(host);
			if (ipAddresses == null || ipAddresses.Length == 0)
			{
				throw new ArgumentException("Cannot resolve host: " + host);
			}
			IPEndPoint endPoint = new IPEndPoint(ipAddresses[0], port);
			_client = new StompClient();
			_client.ConnectedChanged += ClientConnectedChanged;
			Connected = _client.Connected;
			_client.ConnectTo(endPoint);
			_uri = uri;
		}

		private void ClientConnectedChanged(object sender, EventArgs e)
		{
			var client = (StompClient) sender;
			if (client != _client)
			{
				// not interested in this client anymore
				return;
			}

			Connected = _client.Connected;
		}

		public void Close()
		{
			if (_client != null)
			{
				_client.Dispose();
				_client = null;
				_uri = null;
				Connected = false;
			}
		}

		void IDisposable.Dispose()
		{
			Close();
		}

		public SynchronizationContext SynchronizationContext { get; set; }

		public void Publish(object message)
		{
			if (message == null)
			{
				return;
			}

			var type = message.GetType();
			var fullName = type.FullName;
			var destination = QueueName.PublishPrefix + fullName;

			var frame = new StompFrame(StompCommand.Send)
			            	{
			            		Headers =
			            			{
			            				{StompHeader.Destination, destination}
			            			}
			            	};
			frame.Serialize(message);
			_client.SendMessage(frame);
		}

		public ISubscriber<T> CreateSubscriber<T>()
		{
			return new Subscriber<T>(this);
		}

		public IChannel CreateChannel()
		{
			throw new NotImplementedException();
		}

		public IPublisher<T> CreatePublisher<T>()
		{
			throw new NotImplementedException();
		}


		private void RaiseConnectedChanged()
		{
			var sc = SynchronizationContext;
			if (sc == null)
			{
				RaiseConnectedChangedCallback(null);
			}
			else
			{
				sc.Send(RaiseConnectedChangedCallback, null);
			}
		}

		private void RaiseConnectedChangedCallback(object state)
		{
			var connectedChanged = ConnectedChanged;
			if (connectedChanged != null)
			{
				connectedChanged(this, EventArgs.Empty);
			}
		}
	}
}