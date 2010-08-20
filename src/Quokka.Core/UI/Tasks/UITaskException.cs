using System;
using System.Runtime.Serialization;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Base class for all exceptions thrown by the UI Tasks
	/// </summary>
	public class UITaskException : QuokkaException
	{
		public UITaskException() {}
		public UITaskException(string message) : base(message) {}
		public UITaskException(string message, Exception innerException) : base(message, innerException) {}
		protected UITaskException(SerializationInfo info, StreamingContext context) : base(info, context) {}
	}

	/// <summary>
	/// Base class for all exceptions thrown by the UIP framework
	/// </summary>
	public class UITaskInvalidException : UITaskException
	{
		public UITaskInvalidException() { }
		public UITaskInvalidException(string message) : base(message) { }
		public UITaskInvalidException(string message, Exception innerException) : base(message, innerException) { }
		protected UITaskInvalidException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}