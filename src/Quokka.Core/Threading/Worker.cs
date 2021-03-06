﻿#region License

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
using Quokka.UI.Tasks;

namespace Quokka.Threading
{
	///<summary>
	///	Represents an object that knows how to process <see cref = "WorkerAction" /> action objects.
	///</summary>
	public class Worker
	{
		private int _workerActionCount;
		private UITask _task;

		public Worker()
		{
			// get the associated UITask, if any
			_task = UITask.Current;

			// Start with the synchronization context for the curren task.
			// Most applications will have the Worker created on the UI thread.
			SynchronizationContext = SynchronizationContext.Current;
		}

		/// <summary>
		/// 	Raised when the <see cref = "Worker" /> starts processing.
		/// </summary>
		/// <remarks>
		/// 	A <see cref = "Worker" /> can process more than one <see cref = "WorkerAction" /> at 
		/// 	a time. This event will be raised when the first <see cref = "WorkerAction" /> starts.
		/// </remarks>
		public event EventHandler Starting;

		/// <summary>
		/// 	Raised when the <see cref = "Worker" /> stops processing.
		/// </summary>
		/// <remarks>
		/// 	A <see cref = "Worker" /> can process more than one <see cref = "WorkerAction" /> at a time.
		/// 	This event will be raised when the last <see cref = "WorkerAction" /> stops.
		/// </remarks>
		public event EventHandler Stopped;

		///<summary>
		///	A <see cref = "SynchronizationContext" /> for
		///</summary>
		public SynchronizationContext SynchronizationContext { get; set; }

		///<summary>
		///	Are one or more <see cref = "WorkerAction" /> objects being processed.
		///</summary>
		public virtual bool IsBusy
		{
			get { return _workerActionCount > 0; }
		}

		///<summary>
		///	Runs a <see cref = "WorkerAction" />.
		///</summary>
		public virtual void Run(WorkerAction workerAction)
		{
			if (workerAction == null)
			{
				// null action means do nothing
				return;
			}

			workerAction.SynchronizationContext = SynchronizationContext;
			Start();
			if (SynchronizationContext == null)
			{
				// no sync context means run on current thread
				RunHelper(workerAction);
			}
			else
			{
				// we have a sync context so schedule on a worker thread
				ThreadPool.QueueUserWorkItem(RunHelper, workerAction);
			}
		}

		///<summary>
		///	Called to raise the <see cref = "Starting" /> event.
		///</summary>
		protected virtual void OnStarting(EventArgs e)
		{
			if (Starting != null)
			{
				Starting(this, e);
			}
		}

		///<summary>
		///	Called to raise the <see cref = "Stopped" /> event.
		///</summary>
		protected virtual void OnStopped(EventArgs e)
		{
			if (Stopped != null)
			{
				Stopped(this, e);
			}
		}

		private void RunHelper(object state)
		{
			try
			{
				var workerAction = (WorkerAction) state;
				using (SetCurrentTask())
				{
					workerAction.Run();
				}
			}
			finally
			{
				Stop();
			}
		}

		private void Start()
		{
			if (Interlocked.Increment(ref _workerActionCount) == 1)
			{
				try
				{
					PerformAction(() => OnStarting(EventArgs.Empty));
				}
				catch
				{
					// The event handler has thrown an exception, which
					// means that we are not going to proceed with the
					// worker action. Decrement the count to keep it
					// accurate.
					Interlocked.Decrement(ref _workerActionCount);
					throw;
				}
			}
		}

		private void Stop()
		{
			if (Interlocked.Decrement(ref _workerActionCount) == 0)
			{
				PerformAction(() => OnStopped(EventArgs.Empty));
			}
		}

		/// <summary>
		/// 	Perform an action via the SynchronizationContext, if it exists.
		/// </summary>
		private void PerformAction(Action action)
		{
			if (action != null)
			{
				if (SynchronizationContext == null)
				{
					using (SetCurrentTask())
					{
						action();
					}
				}
				else
				{
					SynchronizationContext.Send(obj => {
						using (SetCurrentTask())
						{
							action();
						}
					}, null);
				}
			}
		}

		private IDisposable SetCurrentTask()
		{
			if (_task == null)
			{
				// do nothing -- no current task
				return new DisposableAction(null);
			}
			return UICurrentTask.SetCurrentTask(_task);
		}
	}
}