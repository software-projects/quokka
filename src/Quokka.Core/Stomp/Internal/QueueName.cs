namespace Quokka.Stomp.Internal
{
	public static class QueueName
	{
		/// <summary>
		/// When the destination starts with this prefix, it indicates that
		/// the message should be published to all subscribers of the queue.
		/// </summary>
		public const string PublishPrefix = "/topic/";

		/// <summary>
		/// When the destination starts with this prefix, it indicates that
		/// the message should be delivered to the next available subscriber
		/// of the queue. If there are more than one subscriber, the message
		/// is delivered to one subscriber only.
		/// </summary>
		public const string QueuePrefix = "/queue/";
	}
}
