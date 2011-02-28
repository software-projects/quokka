namespace Quokka.Stomp
{
	/// <summary>
	/// 	Constant strings for STOMP commands.
	/// </summary>
	public static class StompCommand
	{
		public const string Connect = "CONNECT";
		public const string Connected = "CONNECTED";
		public const string Send = "SEND";
		public const string Subscribe = "SUBSCRIBE";
		public const string Unsubscribe = "UNSUBSCRIBE";
		public const string Begin = "BEGIN";
		public const string Commit = "COMMIT";
		public const string Abort = "ABORT";
		public const string Ack = "ACK";
		public const string Disconnect = "DISCONNECT";
		public const string Error = "ERROR";
		public const string Receipt = "RECEIPT";
		public const string Messasge = "MESSAGE";
	}
}