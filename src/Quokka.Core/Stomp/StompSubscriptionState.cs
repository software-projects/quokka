namespace Quokka.Stomp
{
	public enum StompSubscriptionState
	{
		/// <summary>
		/// 	Subscribtion has not been requested.
		/// </summary>
		Unsubscribed,

		/// <summary>
		/// 	Subscription has been requested, but not confirmed.
		/// </summary>
		Subscribing,

		/// <summary>
		/// 	Subscription has been confirmed by the server.
		/// </summary>
		Subscribed,

		/// <summary>
		///		Subscription has been disposed.
		/// </summary>
		Disposed,
	}
}