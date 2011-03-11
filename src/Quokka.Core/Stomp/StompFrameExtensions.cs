namespace Quokka.Stomp
{
	public static class StompFrameExtensions
	{
		public static bool GetBoolean(this StompFrame frame, string header, bool defaultValue)
		{
			var text = frame.Headers[header];
			if (text == null)
			{
				return defaultValue;
			}

			bool value;
			if (!bool.TryParse(text, out value))
			{
				return defaultValue;
			}

			return value;
		}

		public static int GetInt32(this StompFrame frame, string header, int defaultValue)
		{
			var text = frame.Headers[header];
			if (text == null)
			{
				return defaultValue;
			}

			int value;
			if (!int.TryParse(text, out value))
			{
				return defaultValue;
			}

			return value;
		}

		public static long GetInt64(this StompFrame frame, string header, long defaultValue)
		{
			var text = frame.Headers[header];
			if (text == null)
			{
				return defaultValue;
			}

			long value;
			if (!long.TryParse(text, out value))
			{
				return defaultValue;
			}

			return value;
		}
	}
}