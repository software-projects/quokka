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