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
using System.Runtime.Remoting.Messaging;
using Quokka.Diagnostics;
using Quokka.Util;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// This class is used to record the current <see cref="UITask"/>. 
	/// It is for internal use by Quokka.
	/// </summary>
	internal static class UICurrentTask
	{
		private static readonly string Key = Guid.NewGuid().ToString();

		internal static IDisposable SetCurrentTask(UITask task)
		{
			Verify.ArgumentNotNull(task, "task");
			var parentTask = CallContext.GetData(Key) as UITask;
			CallContext.SetData(Key, task);
			return new DisposableAction(() => ClearCurrentTask(parentTask));
		}

		/// <summary>
		/// Returns the current <see cref="UITask"/>
		/// </summary>
		internal static UITask CurrentTask
		{
			get { return CallContext.GetData(Key) as UITask; }
		}

		// Clears out the current task -- called from the disposable action.
		private static void ClearCurrentTask(UITask parentTask)
		{
			CallContext.SetData(Key, parentTask);
			if (parentTask == null)
			{
				CallContext.FreeNamedDataSlot(Key);
			}
		}
	}
}
