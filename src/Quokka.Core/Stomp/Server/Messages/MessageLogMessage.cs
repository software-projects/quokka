using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Quokka.Stomp.Server.Messages
{
	public class MessageLogMessage
	{
		public DateTime SentAt;
		public string Destination;
		public int ContentLength;
	}
}
