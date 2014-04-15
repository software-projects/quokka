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