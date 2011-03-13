using System.Collections.Generic;

namespace Quokka.Stomp.Server.Messages
{
	public class ServerStatusMessage
	{
		public List<MessageQueueStatus> MessageQueues;
		public List<SessionStatus> Sessions;
	}

	public class SessionStatus
	{
		public string SessionId;
		public string ClientId;
		public bool Connected;
		public List<SubscriptionStatus> Subscriptions;
	}

	public class SubscriptionStatus
	{
		public string SubscriptionId;
		public string MessageQueueName;
		public bool AutoAcknowledge;
		public long UnacknowledgedFrameCount;
		public long TotalMessageCount;
	}

	public class MessageQueueStatus
	{
		public string MessageQueueName;
		public int SubscriptionCount;
		public long PendingMessageCount;
		public long TotalMessageCount;
	}
}