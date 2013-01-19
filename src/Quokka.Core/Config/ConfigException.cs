using System;
using System.Runtime.Serialization;

namespace Quokka.Config
{
	/// <summary>
	/// Exception thrown due to errors accessing configuration parameters.
	/// </summary>
	public class ConfigException : QuokkaException
	{
        public ConfigException() { }
        public ConfigException(string message) : base(message) { }
        public ConfigException(string message, Exception innerException) : base(message, innerException) { }
		protected ConfigException(SerializationInfo info, StreamingContext context) : base(info, context) { }

		public ConfigParameter ConfigParameter { get; internal set; }
	}

	/// <summary>
	/// Attempt to write to a read-only configuration parameter.
	/// </summary>
	public class ReadOnlyConfigException : ConfigException
	{
		public ReadOnlyConfigException() { }
		public ReadOnlyConfigException(string message) : base(message) { }
		public ReadOnlyConfigException(string message, Exception innerException) : base(message, innerException) { }
		protected ReadOnlyConfigException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

}
