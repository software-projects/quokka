namespace Quokka.Stomp
{
	/// <summary>
	/// Valid values for the STOMP SUBSCRIBE ack header
	/// </summary>
	public static class StompAck
	{
		public const string Client = "client";
		public const string Auto = "auto";
		public const string ClientIndividual = "client-individual";
	}
}