using System;

namespace Quokka.Diagnostics
{
	[Obsolete("Quokka now uses Common.Logging library for all its logging")]
	public interface ILogManager
	{
		ILogger GetLogger(Type type);
		ILogger GetLogger(string name);
	}
}