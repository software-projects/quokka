namespace Quokka.Events
{
	/// <summary>
	/// Provides options for how the event subscription will refer to
	/// the action delegate.
	/// </summary>
	/// <remarks>
	/// This could have been a boolean value, because it is very unlikely
	/// to ever have more than two values. (Is there a CLI enumeration that
	/// does the same job that we could use?)
	/// </remarks>
	public enum ReferenceOption
	{
		/// <summary>
		/// The event subscription will use a weak reference to the
		/// action delegate. The subscription will lapse if the action
		/// delegate is garbage collected.
		/// </summary>
		WeakReference,

		/// <summary>
		/// The event subscription will use a normal (ie strong) reference
		/// to the action delegate. This will prevent the action delegate
		/// from being garbage collected until the event subscription is
		/// unsubscribed.
		/// </summary>
		StrongReference
	}
}