using System;

namespace Quokka.Stomp.Internal
{
	public static class ExpiresTextUtils
	{
		public const string ExpiresFormat = "yyyyMMddTHHmmssZ";

		public static string ToString(DateTimeOffset dateTime)
		{
			return dateTime.UtcDateTime.ToString(ExpiresFormat);
		}

		public static int Compare(string expires1, string expires2)
		{
			return StringComparer.OrdinalIgnoreCase.Compare(expires1, expires2);
		}
	}
}