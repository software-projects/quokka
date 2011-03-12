using System;
using Quokka.Diagnostics;

namespace Quokka.Stomp.Transport
{
	/// <summary>
	/// 	A subclass of <see cref = "EventArgs" /> with an <see cref = "Exception" /> as payload.
	/// </summary>
	public class ExceptionEventArgs : EventArgs
	{
		public ExceptionEventArgs(Exception exception)
		{
			Exception = Verify.ArgumentNotNull(exception, "exception");
		}

		public Exception Exception { get; private set; }
	}
}