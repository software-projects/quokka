using System;
using System.Runtime.Serialization;

namespace Quokka.WinForms.Startup
{
	public class StartupException : QuokkaException
	{
		public StartupException()
		{
		}

		public StartupException(string message) : base(message)
		{
		}

		public StartupException(string message, Exception innerException) : base(message, innerException)
		{
		}

		protected StartupException(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
	}
}