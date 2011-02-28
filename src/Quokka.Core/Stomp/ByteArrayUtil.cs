namespace Quokka.Stomp
{
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

		public static void SkipNewLine(byte[] data, ref int offset, ref int length)
		{
			if (length == 0)
			{
				return;
			}

			byte @byte = data[offset];

			if (@byte != CarriageReturn && @byte != NewLine)
			{
				return;
			}

			if (@byte == CarriageReturn)
			{
				++offset;
				--length;
				if (length == 0)
				{
					return;
				}

				@byte = data[offset];
			}

			if (@byte == NewLine)
			{
				++offset;
				--length;
			}
		}
	}
}