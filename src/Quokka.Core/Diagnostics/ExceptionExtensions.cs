#region License

// Copyright 2004-2012 John Jeffery <john@jeffery.id.au>
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

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
