#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

#endregion

using System;
using System.Diagnostics;
using System.Reflection;
using Common.Logging;

namespace Quokka.Diagnostics
{
	/// <summary>
	/// Used for internal consistency checking.
	/// </summary>
	public static class Verify
	{
		private static readonly ILog Log = LogManager.GetCurrentClassLogger();

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
				ILog log = LogManager.GetLogger(type);
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