using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Quokka.Stomp
{
	/// <summary>
	/// 	Builds up <see cref = "StompFrame" /> objects from a stream of bytes.
	/// </summary>
	/// <remarks>
	/// 	This class is designed to be thread-safe.
	/// </remarks>
	public class StompFrameBuilder
	{
		/// <summary>
		/// 	Event is raised when one or more <see cref = "StompFrame" /> objects
		/// 	are ready to be processed.
		/// </summary>
		/// <remarks>
		/// 	Use the <see cref = "GetNextFrame" /> method to retrieve <see cref = "StompFrame" /> objects.
		/// </remarks>
		public event EventHandler FrameReady;

		///<summary>
		///	Get the next <see cref = "StompFrame" /> that has been built by this builder object.
		///</summary>
		///<returns>
		///	Returns a <see cref = "StompFrame" /> object, or <c>null</c> if none is available.
		///</returns>
		public StompFrame GetNextFrame()
		{
			lock (_lockObject)
			{
				if (_readyFrames.Count == 0)
				{
					return null;
				}
				return _readyFrames.Dequeue();
			}
		}

		/// <summary>
		/// 	Receive bytes transmitted by the other party, and assemble zero, one or more
		/// 	<see cref = "StompFrame" /> objects from the data. Any data received that does not
		/// 	result in a <see cref = "StompFrame" /> being assembled, will be held over and used
		/// 	in the next call to <see cref = "ReceiveBytes" />.
		/// </summary>
		/// <param name = "data">
		/// 	Array of bytes containing data.
		/// </param>
		/// <param name = "offset">
		/// 	Offset into the <see cref = "data" /> where the received data begins.
		/// </param>
		/// <param name = "length">
		/// 	Number of bytes in the <see cref = "data" /> that have been received.
		/// </param>
		/// <returns>
		/// 	Returns <c>true</c> if one or more <see cref = "StompFrame" /> objects have been
		/// 	assembled as a result of the data received.
		/// </returns>
		/// <remarks>
		/// 	This class takes a copy of any data it needs from the <see cref = "data" /> array,
		/// 	so that it can be reused later.
		/// </remarks>
		public bool ReceiveBytes(byte[] data, int offset, int length)
		{
			bool result;
			lock (_lockObject)
			{
				if (_residue == null)
				{
					// there is no residue from the previous call
					_currentCallback(data, offset, length);
				}
				else
				{
					// there is some residue from the previous call
					var buffer = _residue;
					_residue = null;
					buffer.Append(data, offset, length);
					_currentCallback(buffer.Data, 0, buffer.Length);
				}

				result = _readyFrames.Count > 0;
			}

			if (_framesAdded)
			{
				_framesAdded = false;
				if (FrameReady != null)
				{
					FrameReady(this, EventArgs.Empty);
				}
			}

			return result;
		}

		public StompFrameBuilder()
		{
			_currentCallback = ReadCommandLine;
		}

		private StompFrame _frameUnderConstruction;

		private readonly object _lockObject = new object();

		private readonly Queue<StompFrame> _readyFrames = new Queue<StompFrame>();

		private delegate void ReceiveCallback(byte[] data, int offset, int length);

		private ReceiveCallback _currentCallback;

		private Buffer _residue;

		private bool _framesAdded;

		private void ReadCommandLine(byte[] data, int offset, int length)
		{
			_currentCallback = ReadCommandLine;

			string commandText;

			// This loop provides a bit of extra flexibility. When we are looking for a 
			// command line, we will keep ignoring blank lines (lines containing only spaces)
			// until we find one.
			for (;;)
			{
				int lineLength = ByteArrayUtil.FindLineLength(data, offset, length);
				if (lineLength < 0)
				{
					// This is rare, but there is not enough in the buffer to even read the first line.
					_residue = new Buffer(data, offset, length);
					return;
				}

				// We have the first line, so grab the command text.
				commandText = Encoding.ASCII.GetString(data, offset, lineLength).Trim();

				offset += lineLength;
				length -= lineLength;

				// Skip over the new line
				ByteArrayUtil.SkipNewLine(data, ref offset, ref length);

				if (!string.IsNullOrEmpty(commandText))
				{
					break;
				}
			}

			// At this point we have the command text, and the offset is pointing to the start of the
			// header part of the message.
			_frameUnderConstruction = new StompFrame {Command = commandText};

			ReadHeaders(data, offset, length);
		}

		private void ReadHeaders(byte[] data, int offset, int length)
		{
			_currentCallback = ReadHeaders;

			for (;;)
			{
				int lineLength = ByteArrayUtil.FindLineLength(data, offset, length);
				if (lineLength < 0)
				{
					// There is not a complete line in the buffer. Save it for next time.
					_residue = new Buffer(data, offset, length);
					return;
				}

				// We have a header line.
				// TODO: This code does not support header folding. The STOMP standard
				// does not talk about header folding, but it is common for these types of
				// protocols.
				string line = Encoding.ASCII.GetString(data, offset, lineLength).Trim();

				offset += lineLength;
				length -= lineLength;
				ByteArrayUtil.SkipNewLine(data, ref offset, ref length);

				if (line.Length == 0)
				{
					// we have come to the end of the headers
					ReadBody(data, offset, length);
					break;
				}

				string[] array = line.Split(HeaderKeywordTerminators, 2);
				string keyword = array[0].TrimEnd();
				string value = array.Length > 1 ? array[1].Trim() : string.Empty;
				_frameUnderConstruction.Headers.Add(keyword, value);
			}
		}

		private static readonly char[] HeaderKeywordTerminators = new[] {':'};

		private void ReadBody(byte[] data, int offset, int length)
		{
			_currentCallback = ReadBody;
			int? contentLength = _frameUnderConstruction.GetContentLength();

			if (contentLength == null)
			{
				// We do not know the content length for this frame.
				ReadBodyUntilNull(data, offset, length);
				return;
			}

			if (length < contentLength.Value)
			{
				// Not enough data in the buffer for the body. Leave residue until next time.
				_residue = new Buffer(data, offset, length);
				return;
			}

			// We have the body now. Finish off the frame.
			AddBodyToFrameAndAddFrameToReadyQueue(data, offset, contentLength.Value);
			offset += contentLength.Value;
			length -= contentLength.Value;

			// Look for terminating null
			ReadNull(data, offset, length);
		}

		private void ReadBodyUntilNull(byte[] data, int offset, int length)
		{
			_currentCallback = ReadBodyUntilNull;
			int lengthToNull = ByteArrayUtil.FindLengthToNull(data, offset, length);
			if (lengthToNull < 0)
			{
				// do not have the full body yet
				_residue = new Buffer(data, offset, length);
				return;
			}

			AddBodyToFrameAndAddFrameToReadyQueue(data, offset, lengthToNull);
			offset += lengthToNull;
			length -= lengthToNull;
			ReadNull(data, offset, length);
		}

		private void AddBodyToFrameAndAddFrameToReadyQueue(byte[] data, int offset, int length)
		{
			// TODO: This could do with optimisation. To play it safe, we take a copy 
			// of the data and put it in a buffer for the memory stream. If the data
			// were already in a residue buffer, there would be no need to copy it again.
			var dataCopy = new byte[length];
			Array.Copy(data, offset, dataCopy, 0, length);
			var memoryStream = new MemoryStream(dataCopy);
			_frameUnderConstruction.Body = memoryStream;
			_readyFrames.Enqueue(_frameUnderConstruction);
			_frameUnderConstruction = null;
			_framesAdded = true;
		}

		private void ReadNull(byte[] data, int offset, int length)
		{
			_currentCallback = ReadNull;

			if (length == 0)
			{
				// nothing left to read, so need to wait until next time.
				return;
			}

			if (data[offset] == 0)
			{
				++offset;
				--length;
			}
			else
			{
				// TODO: need some sort of warning here
			}
			ReadCommandLine(data, offset, length);
		}


		// Internal buffer containing data read but not yet used.
		private class Buffer
		{
			// This is the minimum length
			private const int MinimumLength = 4096;
			public byte[] Data;
			public int Length;

			public Buffer(byte[] data, int offset, int length)
			{
				Length = length;
				var arrayLength = Math.Max(MinimumLength, Length);
				Data = new byte[arrayLength];
				Array.Copy(data, offset, Data, 0, Length);
			}

			public void Append(byte[] data, int offset, int length)
			{
				if (Data.Length >= Length + length)
				{
					// the additional data will fit
					Array.Copy(data, offset, Data, Length, length);
					Length += length;
				}
				else
				{
					int newLength = Math.Max(Length*2, Length + length);
					byte[] newData = new byte[newLength];
					Array.Copy(Data, 0, newData, 0, Length);
					Array.Copy(data, offset, newData, Length, length);
					Data = newData;
					Length += length;
				}
			}
		}
	}
}