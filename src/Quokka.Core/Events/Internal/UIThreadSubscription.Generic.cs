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
using System.Threading;
using Quokka.UI;

namespace Quokka.Events.Internal
{
	/// <summary>
	/// Event subscription for the UI thread options.
	/// </summary>
	/// <typeparam name="TPayload"></typeparam>
	internal class UIThreadSubscription<TPayload> : EventSubscription<TPayload>
	{
		public UIThreadSubscription(Event<TPayload> parentEvent, Action<TPayload> action, ThreadOption threadOption,
		                            ReferenceOption referenceOption)
			: base(parentEvent, action, threadOption, referenceOption)
		{
			if (ThreadOption != ThreadOption.UIThread && ThreadOption != ThreadOption.UIThreadPost)
			{
				throw new InvalidOperationException("Incorrect thread option");
			}
		}

		protected override void InvokeAction(Action<TPayload> action, TPayload payload)
		{
			if (ThreadOption == ThreadOption.UIThread)
			{
				UIThread.Send(() => action(payload));
			}
			else
			{
				UIThread.Post(() => action(payload));
			}
		}
	}
}