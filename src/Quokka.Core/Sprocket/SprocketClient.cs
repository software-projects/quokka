using System;
using System.Net;
using System.Threading;
using System.Transactions;
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
			if (message == null || _client == null)
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
			var statusMessage = message as IStatusMessage;
			if (statusMessage != null)
			{
				// This is a status message
				frame.Headers[StompHeader.NonStandard.StatusFor] = statusMessage.GetStatusId();
			}

			frame.Serialize(message);
			TransactionalAwareSendMessage(frame);
		}

		private void TransactionalAwareSendMessage(StompFrame frame)
		{
			if (Transaction.Current == null)
			{
				// not in a transaction
				_client.SendMessage(frame);
			}
			
			if (Transaction.Current != null
				&& Transaction.Current.TransactionInformation.Status == TransactionStatus.Active)
			{
				// this is a 'poor man's' transaction-aware implementation
				Transaction.Current.TransactionCompleted += (sender, e) =>
				                                            	{
				                                            		if (e.Transaction.TransactionInformation.Status ==
				                                            		    TransactionStatus.Committed)
				                                            		{
				                                            			_client.SendMessage(frame);
				                                            		}
				                                            	};
			}
		}

		public bool CanReply
		{
			get
			{
				return CurrentMessage.Frame != null
				       && CurrentMessage.Frame.Headers[StompHeader.NonStandard.ReplyTo] != null;
			}
		}

		public void Reply(object message)
		{
			if (!CanReply)
			{
				throw new InvalidOperationException("Unable to reply");
			}

			if (message == null || _client == null)
			{
				return;
			}

			var destination = CurrentMessage.Frame.Headers[StompHeader.NonStandard.ReplyTo];

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
			return new Channel(this);
		}

		public IPublisher<T> CreatePublisher<T>()
		{
			return new Publisher<T>(this);
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