using System;
using System.Reflection;

namespace Quokka.Diagnostics
{
	public static class ExceptionExtensions
	{
		private static readonly Type[] CorruptedStateExceptionTypes = new[]
		                                                              	{
		                                                              		typeof (OutOfMemoryException),
		                                                              		typeof (AccessViolationException),
																			typeof(StackOverflowException),
		                                                              	};

		public static bool IsCorruptedStateException(this Exception ex)
		{
			if (ex == null)
			{
				return false;
			}

			// Get OutOfMemory exception out of the way quickly, because we don't want
			// to attempt to allocate memory to check for this one.
			if (ex is OutOfMemoryException)
			{
				return true;
			}

			var exceptionType = ex.GetType();

			foreach (var type in CorruptedStateExceptionTypes)
			{
				if (type.IsAssignableFrom(exceptionType))
				{
					return true;
				}
			}

			return false;
		}

		public static string UsefulMessage(this Exception ex)
		{
			if (ex == null)
			{
				return "(null)";
			}

			for (; ; )
			{
				var targetInvocationException = ex as TargetInvocationException;
				if (targetInvocationException == null || ex.InnerException == null)
				{
					return ex.Message;
				}

				ex = ex.InnerException;
			}
		}
	}
}
