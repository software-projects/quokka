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
	public static class UITaskContext
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
		private static UITask CurrentTask
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

		/// <summary>
		/// Is there a <see cref="UITask"/> bound to the current context.
		/// </summary>
		public static bool HasTask
		{
			get { return CurrentTask != null; }
		}

		/// <summary>
		/// Retrieve data stored in the current <see cref="UITask"/> context.
		/// </summary>
		public static object GetData(string key)
		{
			var task = CurrentTask;
			if (task == null)
			{
				return null;
			}
			return task.GetData(key);
		}

		/// <summary>
		/// Save data to the current <see cref="UITask"/> context.
		/// </summary>
		public static bool SetData(string key, object value)
		{
			var task = CurrentTask;
			if (task == null)
			{
				return false;
			}
			task.SetData(key, value);
			return true;
		}
	}
}
