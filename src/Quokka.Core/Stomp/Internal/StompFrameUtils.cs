using System;
using System.Threading;

namespace Quokka.Stomp.Internal
{
	internal static class StompFrameUtils
	{
		public static StompFrame CreateErrorFrame(string message)
		{
			var errorFrame = new StompFrame
			                 	{
			                 		Command = StompCommand.Error,
			                 		Headers =
			                 			{
			                 				{StompHeader.Message, message}
			                 			}
			                 	};

			return errorFrame;
		}

		public static StompFrame CreateErrorFrame(string message, StompFrame inResponseTo)
		{
			var errorFrame = CreateErrorFrame(message);
			var receiptId = inResponseTo.Headers[StompHeader.ReceiptId];
			if (receiptId != null)
			{
				errorFrame.Headers[StompHeader.ReceiptId] = receiptId;
			}
			return errorFrame;
		}

		public static StompFrame CreateMissingHeaderError(string missingHeader, StompFrame frame)
		{
			var message = frame.Command + " is missing mandatory header: " + missingHeader;
			return CreateErrorFrame(message, frame);
		}

		/// <summary>
		/// 	Creates a copy of the frame
		/// </summary>
		/// <param name = "frame">Frame to take a copy of.</param>
		/// <returns>
		/// 	Returns a new frame whose command and headers can be modified without changing the original frame.
		/// 	The body, however, is shared with the original frame.
		/// </returns>
		public static StompFrame CreateCopy(StompFrame frame)
		{
			var copy = new StompFrame
			           	{
			           		Command = frame.Command
			           	};
			copy.Headers.Add(frame.Headers);
			copy.Body = frame.Body;
			return copy;
		}

		private static long _messageId = DateTime.Now.Ticks;

		// Creates a unique, monotonically increasing message id
		// Current implementation would need to generate >10 million messages per second
		// Consistently to give any chance of a repeat number.
		public static long AllocateMessageId(StompFrame frame)
		{
			var messageId = Interlocked.Increment(ref _messageId);
			frame.Headers[StompHeader.MessageId] = messageId.ToString();
			return messageId;
		}
	}
}