#region License

// Copyright 2004-2014 John Jeffery
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
using System.Threading;

namespace Quokka.UI
{
	/// <summary>
	/// Static class for interacting with the main UI thread.
	/// </summary>
	public class UIThread
	{
		/// <summary>
		/// The Synchronization context for the UI thread.
		/// </summary>
		/// <remarks>
		/// This is set very early on in the Bootstrap sequence for a UI application.
		/// Although the property is public writable, the intention is that it should
		/// be set once and then left. Unless you are writing your own bootstrapper, do
		/// not write to this property.
		/// </remarks>
		public static SynchronizationContext SynchronizationContext { get; set; }

		static UIThread()
		{
			// Start off with the current thread's synchronization context.
			// The calling program should initialize this with the sync context
			// of the UI thread.
			SynchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
		}

		/// <summary>
		/// Perform an action on the UI thread synchronously.
		/// </summary>
		/// <param name="action">Action to perform.</param>
		public static void Send(Action action)
		{
			CheckSynchronizationContext();
			SynchronizationContext.Send(state => action(), null);
		}

		/// <summary>
		/// Perform an action on the UI thread asynchronously.
		/// </summary>
		/// <param name="action">Action to perform.</param>
		public static void Post(Action action)
		{
			CheckSynchronizationContext();
			SynchronizationContext.Post(state => action(), null);
		}

		private static void CheckSynchronizationContext()
		{
			if (SynchronizationContext == null)
			{
				const string msg = "UIThread.SynchronizationContext is null, so cannot send/post to the UI thread.";
				throw new InvalidOperationException(msg);
			}
		}
	}
}