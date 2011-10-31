namespace Quokka.Events
{
	/// <summary>
	/// Specifies the thread on which an <see cref="Event{TPayload}"/> subscriber will be called.
	/// </summary>
	public enum ThreadOption
	{
		/// <summary>
		/// The call is performed on the same thread on which the <see cref="Event{TPayload}"/> was published.
		/// </summary>
		PublisherThread,

		/// <summary>
		/// The call is performed synchronously on the UI thread.
		/// </summary>
		UIThread,

		/// <summary>
		/// The call call is perfomed asynchronously on the UI thread.
		/// </summary>
		UIThreadPost,

		/// <summary>
		/// The call is performed asynchronously on a background thread.
		/// </summary>
		BackgroundThread
	}
}