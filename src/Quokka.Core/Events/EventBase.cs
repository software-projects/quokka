namespace Quokka.Events
{
	/// <summary>
	/// Base class for all events.
	/// </summary>
	/// <remarks>
	/// This class contains all functionality that does not rely on the
	/// generic type parameter (<c>TPayload</c>) in the derived <see cref="Event"/> class.
	/// </remarks>
	public class EventBase
	{
		public IEventBroker EventBroker { get; internal set; }
	}
}