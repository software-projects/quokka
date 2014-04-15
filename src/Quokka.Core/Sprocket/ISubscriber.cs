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
using Quokka.Collections;
using Quokka.Diagnostics;

namespace Quokka.Sprocket
{
	public interface ISubscriber<T> : IDisposable
	{
		Type SubscriptionType { get; }
		SynchronizationContext SynchronizationContext { get; set; }
		Action<T> Action { get; set; }
		Func<T, bool> Filter { get; set; }
	}

	public static class SubscriberExtensions
	{
		public static ISubscriber<T> WithAction<T>(this ISubscriber<T> subscriber, Action<T> action)
		{
			Verify.ArgumentNotNull(subscriber, "subscriber");
			subscriber.Action = action;
			return subscriber;
		}

		public static ISubscriber<T> WithFilter<T>(this ISubscriber<T> subscriber, Func<T, bool> filter)
		{
			Verify.ArgumentNotNull(subscriber, "subscriber");
			subscriber.Filter = filter;
			return subscriber;
		}

		public static ISubscriber<T> AddTo<T>(this ISubscriber<T> subscriber, DisposableCollection disposables)
		{
			Verify.ArgumentNotNull(subscriber, "subscriber");
			Verify.ArgumentNotNull(disposables, "disposables");
			disposables.Add(subscriber);
			return subscriber;
		}
	}
}
