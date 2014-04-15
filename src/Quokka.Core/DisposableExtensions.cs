#region License

// Copyright 2004-2013 John Jeffery <john@jeffery.id.au>
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
using System.ComponentModel;
using Quokka.Diagnostics;

namespace Quokka
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
			Verify.ArgumentNotNull(component, "component");
			component.Disposed += (sender, args) => DisposeOf(disposable);
		}

		public static void DisposeWith(this IDisposable disposable, INotifyDisposed component)
		{
			if (disposable == null)
			{
				return;
			}
			Verify.ArgumentNotNull(component, "component");
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