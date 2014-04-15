#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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