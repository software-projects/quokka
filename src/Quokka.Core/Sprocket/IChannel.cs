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
