using System;
using System.ComponentModel;
using Castle.Core.Logging;
using Quokka.Diagnostics;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Event subscription for background thread option.
	/// </summary>
	internal class BackgroundThreadSubscription : EventSubscription
	{
		private static readonly ILogger Log = LoggerFactory.GetCurrentClassLogger();

		public BackgroundThreadSubscription(EventBase parentEvent,
											Action action,
											ThreadOption threadOption,
											ReferenceOption referenceOption)
			: base(parentEvent, action, threadOption, referenceOption)
		{
			if (ThreadOption != ThreadOption.BackgroundThread)
			{
				throw new InvalidOperationException("Invalid thread option");
			}
		}

		protected override void InvokeAction(Action action)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += delegate { action(); };
			worker.RunWorkerCompleted += WorkCompleted;
			worker.RunWorkerAsync();
		}

		private static void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				Log.Error("Unexpected exception in background event publish", e.Error);
			}
		}
	}
}