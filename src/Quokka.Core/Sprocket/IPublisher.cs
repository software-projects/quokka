using System;
using System.Threading;
using Quokka.Collections;
using Quokka.Diagnostics;

namespace Quokka.Sprocket
{
	/// <summary>
	/// Provides a mechanism to efficiently publish to any number
	/// of subscribers.
	/// </summary>
	/// <typeparam name="T">Type of object that will be published.</typeparam>
	/// <remarks>
	/// If there are no subscribers, then the <see cref="Publish"/> method
	/// does not take up any resources, and results in no network traffic.
	/// </remarks>
	public interface IPublisher<T> : IDisposable
	{
		event EventHandler SubscribedChanged;

		ISprocket Sprocket { get; }
		SynchronizationContext SynchronizationContext { get; set; }

		bool Subscribed { get; }
		void Publish(T obj);
	}

	public static class PublisherExtensions
	{
		public static IPublisher<T> AddTo<T>(this IPublisher<T> publisher, DisposableCollection disposables)
		{
			Verify.ArgumentNotNull(publisher, "publisher");
			Verify.ArgumentNotNull(disposables, "disposables");
			disposables.Add(publisher);
			return publisher;
		}

		public static IPublisher<T> WithSynchronizationContext<T>(this IPublisher<T> publisher, SynchronizationContext sc)
		{
			Verify.ArgumentNotNull(publisher, "publisher");
			publisher.SynchronizationContext = sc;
			return publisher;
		}
	}
}
