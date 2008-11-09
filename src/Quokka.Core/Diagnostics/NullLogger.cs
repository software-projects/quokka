namespace Quokka.Diagnostics
{
	using System;

	/// <summary>
	/// Provides a default implementation of the <see cref="ILogger"/> interface.
	/// </summary>
	public static class NullLogger
	{
		private static readonly ILogger _instance;

		/// <summary>
		/// The single instance of the default <see cref="ILogger"/> interface.
		/// </summary>
		public static ILogger Instance
		{
			get { return _instance; }
		}

		static NullLogger()
		{
			_instance = new Implementation();
		}

		private class Implementation : ILogger
		{
			public bool IsDebugEnabled
			{
				get { return false; }
			}

			public bool IsInfoEnabled
			{
				get { return false; }
			}

			public bool IsWarnEnabled
			{
				get { return false; }
			}

			public bool IsErrorEnabled
			{
				get { return false; }
			}

			public bool IsFatalEnabled
			{
				get { return false; }
			}

			public void Debug(object message) {}

			public void Debug(object message, Exception exception) {}

			public void DebugFormat(string format, object arg0) {}

			public void DebugFormat(string format, object arg0, object arg1) {}

			public void DebugFormat(string format, object arg0, object arg1, object arg2) {}

			public void DebugFormat(IFormatProvider provider, string format, params object[] args) {}

			public void DebugFormat(string format, params object[] args) {}

			public void Info(object message) {}

			public void Info(object message, Exception exception) {}

			public void InfoFormat(string format, object arg0) {}

			public void InfoFormat(string format, object arg0, object arg1) {}

			public void InfoFormat(string format, object arg0, object arg1, object arg2) {}

			public void InfoFormat(IFormatProvider provider, string format, params object[] args) {}

			public void InfoFormat(string format, params object[] args) {}

			public void Warn(object message) {}

			public void Warn(object message, Exception exception) {}

			public void WarnFormat(string format, object arg0) {}

			public void WarnFormat(string format, object arg0, object arg1) {}

			public void WarnFormat(string format, object arg0, object arg1, object arg2) {}

			public void WarnFormat(IFormatProvider provider, string format, params object[] args) {}

			public void WarnFormat(string format, params object[] args) {}

			public void Error(object message) {}

			public void Error(object message, Exception exception) {}

			public void ErrorFormat(string format, object arg0) {}

			public void ErrorFormat(string format, object arg0, object arg1) {}

			public void ErrorFormat(string format, object arg0, object arg1, object arg2) {}

			public void ErrorFormat(IFormatProvider provider, string format, params object[] args) {}

			public void ErrorFormat(string format, params object[] args) {}

			public void Fatal(object message) {}

			public void Fatal(object message, Exception exception) {}

			public void FatalFormat(string format, object arg0) {}

			public void FatalFormat(string format, object arg0, object arg1) {}

			public void FatalFormat(string format, object arg0, object arg1, object arg2) {}

			public void FatalFormat(IFormatProvider provider, string format, params object[] args) {}

			public void FatalFormat(string format, params object[] args) {}
		}
	}
}