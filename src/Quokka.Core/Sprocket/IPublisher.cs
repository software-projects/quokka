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
