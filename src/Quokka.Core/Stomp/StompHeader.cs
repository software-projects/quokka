namespace Quokka.Stomp
{
	public static class StompHeader
	{
		public const string ContentLength = "content-length";
		public const string ContentType = "content-type";
		public const string Destination = "destination";
		public const string Receipt = "receipt";
		public const string MessageId = "message-id";
		public const string ReceiptId = "receipt-id";
		public const string Login = "login";
		public const string Passcode = "passcode";
		public const string Session = "session";
		public const string Ack = "ack";
		public const string Transaction = "transaction";
		public const string Message = "message";
		public const string Id = "id";
		public const string Subscription = "subscription";

		public static class NonStandard
		{
			/// <summary>
			/// 	Specifies time that this message expires yyyymmddThhmmssZ
			/// </summary>
			public const string Expires = "expires";

			/// <summary>
			/// 	Used during unit testing to simulate losing a connection
			/// 	to the server, and attempting to re-establish the session.
			/// </summary>
			public const string KeepSession = "keep-session";

			/// <summary>
			///		When the message payload contains a serialized CLR object,
			///		this header provides enough information for the receiver
			///		to find a serializer object for it. The content-type header
			///		provides information on how the object has been serialized
			///		(application/xml, application/json, etc).
			/// </summary>
			public const string ClrType = "clr-type";

			/// <summary>
			///		Text string that can be helpful in identifying the client
			/// </summary>
			public const string ClientId = "client-id";

			/// <summary>
			/// Indicates that this is a status message. When a message is published
			/// it goes to all subscribers. When a subscriber subsribes, it receives
			/// the last status message published for each separate value of 'status-for'.
			/// </summary>
			/// <example>
			/// If you are publishing the current connection status to a number of different
			/// hosts, you could set the 'status-for' message to the name of the host. When
			/// a subscriber connects, they will immediately receive a list of the last
			/// status message published for each of the hosts.
			/// </example>
			public const string StatusFor = "status-for";

			/// <summary>
			/// Provides a queue name that for receiving replies.
			/// </summary>
			public const string ReplyTo = "reply-to";
		}
	}
}