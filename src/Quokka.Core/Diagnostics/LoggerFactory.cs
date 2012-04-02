using System;
using System.Diagnostics;
using Castle.Core.Logging;

namespace Quokka.Diagnostics
{
	/// <summary>
	/// Static class to provide access to Castle <see cref="ILoggerFactory"/> singleton
	/// instance to instances that are not directly created by the IoC container.
	/// </summary>
	public static class LoggerFactory
	{
		private static ILoggerFactory _loggerFactory = new NullLogFactory();

		public static ILogger GetCurrentClassLogger()
		{
			var frame = new StackFrame(1, false);
			return _loggerFactory.Create(frame.GetMethod().DeclaringType);
		}

		public static ILogger GetLogger(string name)
		{
			return _loggerFactory.Create(name);
		}

		public static ILogger GetLogger(Type type)
		{
			return _loggerFactory.Create(type);
		}

		public static void SetLoggerFactory(ILoggerFactory loggerFactory)
		{
			if (loggerFactory == null)
			{
				loggerFactory = new NullLogFactory();
			}
			_loggerFactory = loggerFactory;
		}

		/// <summary>
		/// Has the LoggerFactory been configured.
		/// </summary>
		/// <value>
		/// If <c>true</c>, then the LoggerFactory has been configured, otherwise it
		/// creates loggers that do nothing.
		/// </value>
		public static bool IsConfigured
		{
			get { return (_loggerFactory as NullLogFactory) == null; }
		}
	}
}