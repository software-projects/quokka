using System;
using System.ComponentModel;
using Quokka.Diagnostics;

namespace Quokka.Util
{
	public static class DisposableExtensions
	{
		/// <summary>
		/// Causes this object to be disposed when the <see cref="Component"/> is disposed.
		/// </summary>
		/// <param name="disposable">
		///		The <see cref="IDisposable"/> object that</param> should be disposed when
		///		the <paramref name="component"/> is disposed.
		/// <param name="component">
		///		When this component is disposed, the <paramref name="disposable"/> object
		///		should be disposed.
		/// </param>
		/// <remarks>
		///		This is useful for ensuring that objects are disposed when a Windows Forms
		///		control is disposed.
		/// </remarks>
		public static void DisposeWith(this IDisposable disposable, Component component)
		{
			if (disposable == null)
			{
				return;
			}

			component.Disposed += (sender, args) => DisposeOf(disposable);
		}

		private static void DisposeOf(IDisposable disposable)
		{
			try
			{
				disposable.Dispose();
			}
			catch (ObjectDisposedException ex)
			{
				// not supposed to happen, but sometimes does
				var logger = LoggerFactory.GetCurrentClassLogger();
				var msg = string.Format("Object of type {0} raised ObjectDisposedException during disposal",
				                        disposable.GetType());
				logger.Warn(msg, ex);
			}
			catch (Exception ex)
			{
				var logger = LoggerFactory.GetCurrentClassLogger();
				var msg = string.Format("Object of type {0} raised an exception during disposal: {1}",
				                        disposable.GetType(),
				                        ex.Message);
				logger.Error(msg, ex);
			}
		}
	}
}