namespace Quokka.Stomp.Internal
{
	public static class QueueName
	{
		/// <summary>
		/// When the destination starts with this prefix, it indicates that
		/// the message should be published to all subscribers of the queue.
		/// </summary>
		public const string PublishPrefix = "/publish/";
	}
}
