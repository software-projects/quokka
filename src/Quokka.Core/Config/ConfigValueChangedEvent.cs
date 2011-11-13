using Quokka.Events;

namespace Quokka.Config
{
	/// <summary>
	/// This event is raised when a configuration parameter is changed.
	/// </summary>
	public class ConfigValueChangedEvent : Event<string>
	{
	}
}
