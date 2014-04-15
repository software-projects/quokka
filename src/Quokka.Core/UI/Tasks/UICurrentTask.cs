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
