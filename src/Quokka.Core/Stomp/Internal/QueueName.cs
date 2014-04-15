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
