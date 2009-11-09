using System;
using System.Collections.Generic;

namespace Quokka.Events
{
	public static class EventSubscriptionExtensions
	{
		/// <summary>
		/// Add to a disposable collection
		/// </summary>
		/// <param name="eventSubscription"></param>
		/// <param name="collection"></param>
		/// <returns></returns>
		/// <remarks>
		/// This makes it nice and easy to add to a disposable collection using a fluent interface:
		/// <example>
		/// <code>
		/// EventBroker.GetEvent&lt;SomeEvent&gt;()
		///     .Subscribe(SomeAction)
		///     .SetFilter(SomeFilter)
		///     .AddTo(collectionOfDisposableObjects);
		/// </code>
		/// </example>
		/// </remarks>
		public static IEventSubscription AddTo(this IEventSubscription eventSubscription, ICollection<IDisposable> collection)
		{
			collection.Add(eventSubscription);
			return eventSubscription;
		}

		public static IEventSubscription AddTo(this IEventSubscription eventSubscription,
		                                       ICollection<IEventSubscription> collection)
		{
			collection.Add(eventSubscription);
			return eventSubscription;
		}

		public static IEventSubscription<TPayload> AddTo<TPayload>(
			this IEventSubscription<TPayload> eventSubscription,
			ICollection<IDisposable> collection)
		{
			collection.Add(eventSubscription);
			return eventSubscription;
		}

		public static IEventSubscription<TPayload> AddTo<TPayload>(
			this IEventSubscription<TPayload> eventSubscription,
			ICollection<IEventSubscription> collection)
		{
			collection.Add(eventSubscription);
			return eventSubscription;
		}

		public static IEventSubscription<TPayload> AddTo<TPayload>(
			this IEventSubscription<TPayload> eventSubscription,
			ICollection<IEventSubscription<TPayload>> collection)
		{
			collection.Add(eventSubscription);
			return eventSubscription;
		}
	}
}