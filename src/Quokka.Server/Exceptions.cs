using System;
using System.Runtime.Serialization;

namespace Quokka.Server
{
	/// <summary>
	/// Base class for exceptions thrown from Quokka.Server assembly.
	/// </summary>
	public class QuokkaServerException: QuokkaException
	{
		public QuokkaServerException(string message) : base(message) { }
		public QuokkaServerException(string message, Exception ex) : base(message, ex) { }
		public QuokkaServerException() : base("Sprocket Exception") { }
		protected QuokkaServerException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
	}

	public class CannotStartException : QuokkaServerException
	{
		public CannotStartException(string message) : base(message) { }
		public CannotStartException(string message, Exception ex) : base(message, ex) { }
		public CannotStartException() : base("Cannot Start Exception") { }
		protected CannotStartException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
	}

	public class AnotherInstanceRunningException : CannotStartException
	{
		public AnotherInstanceRunningException(string message) : base(message) { }
		public AnotherInstanceRunningException(string message, Exception ex) : base(message, ex) { }
		public AnotherInstanceRunningException() : base("Another instance of this program is already running") { }
		protected AnotherInstanceRunningException(SerializationInfo info, StreamingContext ctx) : base(info, ctx) { }
	}
}
