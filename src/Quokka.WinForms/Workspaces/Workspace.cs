using System;
using System.Collections.Generic;
using Quokka.UI;
using Quokka.Uip;

namespace Quokka.WinForms.Workspaces
{
	public abstract class Workspace : IWorkspace
	{
		private readonly List<UipTask> _tasks = new List<UipTask>();

		public IEnumerable<UipTask> Tasks
		{
			get { return _tasks; }
		}

		public void StartTask(UipTask task)
		{
			if (_tasks.Contains(task) || task.IsRunning || task.IsComplete)
			{
				throw new InvalidOperationException("Task already started");
			}

			IUipViewManager viewManager = GetViewManager(task);
			task.Start(viewManager);
		}

		protected abstract IUipViewManager GetViewManager(UipTask task);
	}
}