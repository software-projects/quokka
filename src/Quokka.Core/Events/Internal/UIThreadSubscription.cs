using System;
using Quokka.UI;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Event subscription for the UI thread options.
	/// </summary>
	internal class UIThreadSubscription : EventSubscription
	{
		public UIThreadSubscription(EventBase parentEvent, Action action, ThreadOption threadOption,
		                            ReferenceOption referenceOption)
			: base(parentEvent, action, threadOption, referenceOption)
		{
			if (ThreadOption != ThreadOption.UIThread && ThreadOption != ThreadOption.UIThreadPost)
			{
				throw new InvalidOperationException("Incorrect thread option");
			}
		}

		protected override void InvokeAction(Action action)
		{
			if (ThreadOption == ThreadOption.UIThread)
			{
				UIThread.Send(action);
			}
			else
			{
				UIThread.Post(action);
			}
		}
	}
}