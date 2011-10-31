using System;
using Quokka.Stomp;

namespace Quokka.Sprocket
{
	/// <summary>
	/// Additional information about the message currently being processed by Sprocket.
	/// </summary>
	public static class CurrentMessage
	{
		[ThreadStatic]
		private static StompFrame _stompFrame;

		/// <summary>
		/// The received STOMP frame.
		/// </summary>
		public static StompFrame Frame
		{
			get { return _stompFrame; }
			set { _stompFrame = value; }
		}
	}
}
