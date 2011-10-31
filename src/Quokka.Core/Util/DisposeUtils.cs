using System;

namespace Quokka.Util
{
	/// <summary>
	/// Not quite ready to make this one public yet
	/// </summary>
	internal static class DisposeUtils
	{
		public static void DisposeOf(object obj)
		{
			var disposable = obj as IDisposable;
			if (disposable != null)
			{
				try
				{
					disposable.Dispose();
				}
				catch (ObjectDisposedException) {}
			}
		}

		public static T DisposeOf<T>(ref T obj) where T : class, IDisposable
		{
			if (obj != null)
			{
				try
				{
					obj.Dispose();
				}
				catch (ObjectDisposedException)
				{
					// IDisposables should not throw this if Dispose is called twice, but many do
				}
			}
			return null;
		}
	}
}