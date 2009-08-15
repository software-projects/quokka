using System;
using System.ComponentModel;
using Common.Logging;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Event subscription for background thread option.
	/// </summary>
	/// <typeparam name="TPayload"></typeparam>
	internal class BackgroundThreadSubscription<TPayload> : EventSubscription<TPayload>
	{
		private static readonly ILog log = LogManager.GetCurrentClassLogger();

		public BackgroundThreadSubscription(EventBase parentEvent,
		                                    Action<TPayload> action,
		                                    ThreadOption threadOption,
		                                    ReferenceOption referenceOption)
			: base(parentEvent, action, threadOption, referenceOption)
		{
			if (ThreadOption != ThreadOption.BackgroundThread)
			{
				throw new InvalidOperationException("Invalid thread option");
			}
		}

		protected override void InvokeAction(Action<TPayload> action, TPayload payload)
		{
			BackgroundWorker worker = new BackgroundWorker();
			worker.DoWork += delegate { action(payload); };
			worker.RunWorkerCompleted += WorkCompleted;
			worker.RunWorkerAsync(payload);
		}

		private static void WorkCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (e.Error != null)
			{
				log.Error("Unexpected exception in background event publish", e.Error);
			}
		}
	}
}