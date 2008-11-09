using System;
using System.Diagnostics;
using System.Reflection;
using Quokka.Uip;

namespace Quokka.Diagnostics
{
	/// <summary>
	/// Used internally for logging. The calling application should register a log manager
	/// in the parent service provider for this to work.
	/// </summary>
	/// <remarks>
	/// Maybe it would be better to use Common.Logging one day.
	/// </remarks>
	internal class LogManager
	{
		public static ILogger GetLogger()
		{
			if (UipTask.ParentServiceProvider == null)
				return NullLogger.Instance;

			ILogManager logManager = (ILogManager) UipTask.ParentServiceProvider.GetService(typeof (ILogManager));
			if (logManager == null)
				return NullLogger.Instance;

			// get the type from the calling method
			StackFrame stackFrame = new StackFrame(1);
			MethodBase method = stackFrame.GetMethod();
			Type type = method.DeclaringType;
			return logManager.GetLogger(type);
		}
	}
}