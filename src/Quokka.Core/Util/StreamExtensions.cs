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
