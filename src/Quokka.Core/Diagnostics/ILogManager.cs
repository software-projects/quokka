using System;

namespace Quokka.Diagnostics
{
	public interface ILogManager
	{
		ILogger GetLogger(Type type);
	}
}