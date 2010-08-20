using System;
using System.Collections.Generic;

namespace Quokka.UI.Tasks
{
	/// <summary>
	/// Provides in-memory storage of <see cref="TaskBuilder"/> objects.
	/// </summary>
	internal static class TaskTemplateStorage
	{
		private static readonly Dictionary<Type, TaskBuilder> TaskBuilders = new Dictionary<Type, TaskBuilder>();
		private static readonly object LockObject = new object();

		public static TaskBuilder FindForType(Type taskType)
		{
			TaskBuilder taskBuilder;
			lock (LockObject)
			{
				TaskBuilders.TryGetValue(taskType, out taskBuilder);
			}
			return taskBuilder;
		}

		public static void Add(TaskBuilder taskBuilder)
		{
			lock (LockObject)
			{
				TaskBuilders[taskBuilder.TaskType] = taskBuilder;
			}
		}
	}
}