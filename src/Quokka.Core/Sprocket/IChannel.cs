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
	/// A channel is used for sending commands and receiving responses.
	/// </summary>
	public interface IChannel : IDisposable
	{
		ISprocket Sprocket { get; }
		SynchronizationContext SynchronizationContext { get; set; }
		TimeSpan Timeout { get; set; }
		Action TimeoutAction { get; set; }

		void Send(object message);
		IChannel HandleResponse<T>(Action<T> action);
	}

	public static class ChannelExtensions
	{
		public static IChannel WithSynchronizationContext(this IChannel channel, SynchronizationContext sc)
		{
			Verify.ArgumentNotNull(channel, "channel");
			channel.SynchronizationContext = sc;
			return channel;
		}

		public static IChannel HandleTimeout(this IChannel channel, TimeSpan timeSpan, Action action)
		{
			channel.Timeout = timeSpan;
			channel.TimeoutAction = action;
			return channel;
		}

		public static IChannel HandleTimeout(this IChannel channel, int seconds, Action action)
		{
			channel.Timeout = TimeSpan.FromSeconds(seconds);
			channel.TimeoutAction = action;
			return channel;
		}

		public static IChannel AddTo(this IChannel channel, DisposableCollection disposables)
		{
			Verify.ArgumentNotNull(disposables, "disposables");
			Verify.ArgumentNotNull(channel, "channel");
			disposables.Add(channel);
			return channel;
		}
	}
}
