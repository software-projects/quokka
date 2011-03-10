using System;
using Quokka.Diagnostics;

namespace Quokka.Stomp
{
	public class StompMessageEventArgs : EventArgs
	{
		public StompFrame Message { get; private set; }

		public StompMessageEventArgs(StompFrame message)
		{
			Message = Verify.ArgumentNotNull(message, "message");
		}
	}
}