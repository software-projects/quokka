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
using System.Diagnostics;
using System.Reflection;
using Castle.Core.Logging;

namespace Quokka.Diagnostics
{
	/// <summary>
	/// Used for internal consistency checking.
	/// </summary>
	public static class Verify
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();

		public static T ArgumentNotNull<T>(T param, string paramName) where T : class
		{
			if (param == null)
			{
				throw new ArgumentNullException(paramName);
			}
			return param;
		}

		public static void ArgumentNotNull<T>(T param, string paramName, out T copy)
		{
			object obj = param;
			if (obj == null)
			{
				throw new ArgumentNullException(paramName);
			}
			copy = param;
		}

		public static void IsNotNull(object obj)
		{
			if (obj != null)
			{
				return;
			}

			try
			{
				// Get the type from the calling method
				StackFrame stackFrame = new StackFrame(1);
				MethodBase method = stackFrame.GetMethod();
				Type type = method.DeclaringType;

				// Log details using the logger of the calling type. This will make it much easier to diagnose where
				// the NullReferenceException is thrown from.
				string message = String.Format("Verify.IsNotNull failed in method {0}, type {1}", method.Name, type.FullName);
				ILogger log = LoggerFactory.GetLogger(type);
				log.Error(message);
			}
			catch (Exception ex)
			{
				Log.Warn("Failed to acquire stack frame in Verify.IsNotNull."
				         + " Unable to log details of where the NullReferenceException is thrown from", ex);
			}

			throw new NullReferenceException();
		}
	}
}