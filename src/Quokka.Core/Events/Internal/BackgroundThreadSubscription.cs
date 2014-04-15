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