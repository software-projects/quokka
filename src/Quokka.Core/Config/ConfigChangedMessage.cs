using Quokka.Config.Implementation;
using Quokka.Events;

namespace Quokka.Config
{
	/// <summary>
	/// Publish this message when a config parameter value is changed.
	/// </summary>
	/// <remarks>
	/// The <see cref="CachingConfigBase"/> implementation subscribes
	/// to this message and clears relevant cached values when it receives
	/// this message.
	/// </remarks>
	public class ConfigChangedMessage
	{
		/// <summary>
		/// Name of the parameter whose value has changed.
		/// </summary>
		/// <remarks>
		/// If a message is transmitted with <c>null</c> or an
		/// empty string for this value, then all cached configuration
		/// values will be cleared.
		/// </remarks>
		public string ParamName { get; set; }

		/// <summary>
		/// The type of the parameter that has changed.
		/// </summary>
		/// <remarks>
		/// This value is not used by <see cref="CachingConfigBase"/>, but it might
		/// be useful for other listeners to this message.
		/// </remarks>
		public string ParamType { get; set; }

		/// <summary>
		/// The new value of the parameter that has changed.
		/// </summary>
		/// <remarks>
		/// This value is not used by <see cref="CachingConfigBase"/>, but it might
		/// be useful for other listeners to this message.
		/// </remarks>
		public string ParamValue { get; set; }
	}
}
