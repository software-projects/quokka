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
