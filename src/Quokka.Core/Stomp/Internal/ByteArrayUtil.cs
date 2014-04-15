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

namespace Quokka.Stomp
{
	// Various utility methods for handling the content of an array of bytes
	public class ByteArrayUtil
	{
		// ReSharper disable InconsistentNaming
		private const byte CarriageReturn = 0x0d;
		private const byte NewLine = 0x0a;
		// ReSharper restore InconsistentNaming

		public static int FindLineLength(byte[] data, int offset, int length)
		{
			int endIndex = offset + length;
			for (int index = offset; index < endIndex; ++index)
			{
				var @byte = data[index];
				if (@byte == NewLine)
				{
					// NewLine by itself
					return index - offset;
				}
				if (@byte == CarriageReturn)
				{
					// Found a carriage return. If not enough room for a newline, then not found
					if (index == endIndex - 1)
					{
						return -1;
					}
					var nextByte = data[index + 1];
					if (nextByte == NewLine)
					{
						return index - offset;
					}
				}
			}

			return -1;
		}

		public static int FindLengthToNull(byte[] data, int offset, int length)
		{
			int endIndex = offset + length;

			for (int index = offset; index < endIndex; ++index)
			{
				var @byte = data[index];
				if (@byte == 0)
				{
					return index - offset;
				}
			}

			return -1;
		}

		/// <summary>
		/// Modifies offset and length to skip new lines in the data
		/// </summary>
		/// <param name="data">Data</param>
		/// <param name="offset">Offset</param>
		/// <param name="length">Length</param>
		/// <returns>
		/// Returns <c>true</c> if a newline was skipped, <c>false</c> otherwise.
		/// </returns>
		public static bool SkipNewLine(byte[] data, ref int offset, ref int length)
		{
			if (length == 0)
			{
				return false;
			}

			byte @byte = data[offset];

			if (@byte != CarriageReturn && @byte != NewLine)
			{
				return false;
			}

			if (@byte == CarriageReturn)
			{
				++offset;
				--length;
				if (length == 0)
				{
					return true;
				}

				@byte = data[offset];
			}

			if (@byte == NewLine)
			{
				++offset;
				--length;
				return true;
			}

			return false;
		}
	}
}