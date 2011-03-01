namespace Quokka.Stomp
{
	/// <summary>
	/// 	Constant strings for STOMP commands.
	/// </summary>
	public static class StompCommand
	{
		// Messages Client to Server
		public const string Connect = "CONNECT";
		public const string Send = "SEND";
		public const string Subscribe = "SUBSCRIBE";
		public const string Unsubscribe = "UNSUBSCRIBE";
		public const string Begin = "BEGIN";
		public const string Commit = "COMMIT";
		public const string Abort = "ABORT";
		public const string Ack = "ACK";
		public const string Nack = "NACK";
		public const string Disconnect = "DISCONNECT";

		// Messages Server to Client
		public const string Connected = "CONNECTED";
		public const string Messasge = "MESSAGE";
		public const string Error = "ERROR";
		public const string Receipt = "RECEIPT";
	}
}