namespace Quantum.Constants
{
	/// <summary>
	/// Identifying the purpose of a configuration parameter.
	/// </summary>
	public static class ConfigParameterType
	{
		/// <summary>
		/// General purpose string.
		/// </summary>
		public const string String = "String";

		/// <summary>
		/// 32 bit signed integer
		/// </summary>
		public const string Int32 = "Int32";

		/// <summary>
		/// Boolean (true or false)
		/// </summary>
		public const string Boolean = "Boolean";

		/// <summary>
		/// String representing a URL (http, ftp, https)
		/// </summary>
		public const string Url = "URL";

		/// <summary>
		/// String representing a password (value will be elided in user interface)
		/// </summary>
		public const string Password = "Password";

		/// <summary>
		/// String representing a directory on the quantum server.
		/// </summary>
		public const string Directory = "Directory";

		/// <summary>
		/// Represents a date without any time component.
		/// </summary>
		public const string Date = "Date";

		/// <summary>
		/// Represents a period of time. Useful for configuring timeouts.
		/// </summary>
		public const string TimeSpan = "TimeSpan";

		// TODO: Other types requiring implementation include:
		// * TimeSpan (would be very useful for timeouts, but need to parse strings like "15m" and "500ms")
		// * DateTime
		// * DateTimeOffset
	}
}