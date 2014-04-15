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

#if !NET40
using System;
using Quokka.Diagnostics;

// ReSharper disable CheckNamespace
namespace System.IO
{
	/// <summary>
	/// Implements the .NET 4.0 Stream.CopyTo methods as extension methods.
	/// </summary>
	internal static class StreamCopyToExtensions
	{
		public static void CopyTo(this Stream @this, Stream destination)
		{
			CopyTo(@this, destination, 8192);
		}

		public static void CopyTo(this Stream @this, Stream destination, int bufferSize)
		{
			Verify.ArgumentNotNull(@this, "this");
			Verify.ArgumentNotNull(destination, "destination");
			if (bufferSize < 8192)
			{
				bufferSize = 8192;
			}

			var buffer = new byte[bufferSize];
			for (;;)
			{
				var byteCount = @this.Read(buffer, 0, buffer.Length);
				if (byteCount == 0)
				{
					break;
				}
				destination.Write(buffer, 0, byteCount);
			}
		}
	}
}
#endif // NET40
