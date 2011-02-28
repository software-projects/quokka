namespace Quokka.Stomp
{
	public static class StompFrameExtensions
	{
		public static int? GetContentLength(this StompFrame frame)
		{
			int contentLength;
			string text = frame.Headers[StompHeader.ContentLength];
			if (text == null)
			{
				return null;
			}

			if (int.TryParse(text, out contentLength))
			{
				return contentLength;
			}

			// TODO: should log this somehow?
			return null;
		}
	}
}