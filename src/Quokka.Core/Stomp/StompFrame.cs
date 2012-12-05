using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Quokka.Diagnostics;

namespace Quokka.Stomp
{
	/// <summary>
	/// 	Represents a message that is exchanged between STOMP clients and the STOMP message broker.
	/// </summary>
	/// <remarks>
	/// 	A STOMP message consists of a command, one or more header/value pairs, and a body. For
	///		more information on the format of a STOMP message see http://stomp.github.com
	/// </remarks>
	public class StompFrame
	{
		public static readonly StompFrame HeartBeat = new StompFrame(string.Empty);

		public StompFrame() : this(StompCommand.Send)
		{
			// Default command is SEND, because this is the only kind of
			// command that application code should create.
		}

		public StompFrame(string command)
		{
			Command = Verify.ArgumentNotNull(command, "command");
			Headers = new StompHeaderCollection();
		}

		/// <summary>
		/// 	The <see cref = "StompCommand" /> for this frame.
		/// </summary>
		public string Command { get; set; }

		/// <summary>
		/// Does this frame represent a heart beat message. (Single newline in STOMP protocol)
		/// </summary>
		public bool IsHeartBeat
		{
			get { return ReferenceEquals(this, HeartBeat) || String.IsNullOrEmpty(Command); }
		}

		/// <summary>
		/// 	Collection of name/value pairs, which represent the STOMP frame headers.
		/// </summary>
		/// <remarks>
		/// 	This property is settable, which is not always common for collection objects.
		/// 	The reason it is settable is to make it easy to use C# initializers when
		/// 	creating the frame. Setting this property to <c>null</c> is unsupported
		/// 	and will result in an exception.
		/// </remarks>
		/// <exception cref = "ArgumentNullException">
		/// 	Attempt to set the property value to <c>null</c>.
		/// </exception>
		public StompHeaderCollection Headers
		{
			get { return _headers; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value", "Value of Headers property cannot be null");
				}
				_headers = value;
			}
		}

		private StompHeaderCollection _headers;

		public MemoryStream Body { get; set; }

		public string BodyText
		{
			get
			{
				if (Body == null)
				{
					return null;
				}

				Body.Seek(0, SeekOrigin.Begin);
				var reader = new StreamReader(Body, Encoding.UTF8);
				return reader.ReadToEnd();
			}

			set
			{
				Body = new MemoryStream();
				var writer = new StreamWriter(Body, Encoding.UTF8);
				writer.Write(value);
				writer.Flush();
			}
		}

		/// <summary>
		/// 	Convert the <see cref = "StompFrame" /> into a byte array for transmission.
		/// </summary>
		/// <returns>
		/// 	Returns a byte array of bytes to transmit on the wire.
		/// </returns>
		/// <exception cref = "InvalidOperationException">
		/// 	Thrown when the <see cref = "Command" /> property has not been set.
		/// </exception>
		/// <remarks>
		/// 	Calling this method will result in the "content-length" header being set to the
		/// 	appropriate value prior to creating the byte array. If the <see cref = "Body" />
		/// 	is <c>null</c> or empty, the "content-length" header will still be set (and its
		/// 	value will be "0").
		/// </remarks>
		public byte[] ToArray()
		{
			if (IsHeartBeat)
			{
				return new byte[] {10};
			}

			// Before transmitting, always set the content-length header.
			// Note that this overwrites anything put there by the calling program.
			bool hasBody = (Body != null && Body.Length > 0);
			if (hasBody)
			{
				Headers[StompHeader.ContentLength] = Body.Length.ToString();
			}
			else
			{
				Headers[StompHeader.ContentLength] = "0";
			}

			using (var stream = new MemoryStream())
			{
				{
					// We do not want to close the writer when we finish, because
					// we do not want to close the underlying stream.
					// Note also that the StreamWriter is created without specifying the encoding.
					// This means that the stream will write UTF-8, which is what is wanted, but
					// will not prepend a BOM (byte order mark), which is also what we want. If you
					// explicitly specify UTF8, you will get the three byte BOM at the beginning.
					var writer = new StreamWriter(stream);
					writer.WriteLine(Command);
					if (Headers != null)
					{
						foreach (var key in Headers.AllKeys)
						{
							writer.Write(key);
							writer.Write(":");
							writer.WriteLine(Headers[key]);
						}
					}
					writer.WriteLine();
					writer.Flush();
				}

				// Finished with writing ASCII text, now.
				if (hasBody)
				{
					Body.WriteTo(stream);
				}

				// Finish with null (^@) character
				stream.WriteByte(0);

				return stream.ToArray();
			}
		}

		public override string ToString()
		{
			if (IsHeartBeat)
			{
				return "(heart-beat)";
			}
			var sb = new StringBuilder(Command);
			switch (Command)
			{
				case StompCommand.Abort:
				case StompCommand.Begin:
				case StompCommand.Commit:
					AppendHeader(sb, StompHeader.Transaction);
					break;
				case StompCommand.Ack:
				case StompCommand.Nack:
					AppendHeader(sb, StompHeader.Subscription);
					AppendHeader(sb, StompHeader.Subscription);
					AppendHeaderIfPresent(sb, StompHeader.Transaction);
					break;
				case StompCommand.Connect:
				case StompCommand.Stomp:
					AppendHeaderIfPresent(sb, StompHeader.HeartBeat);
					break;
				case StompCommand.Connected:
					AppendHeader(sb, StompHeader.Session);
					AppendHeaderIfPresent(sb, StompHeader.HeartBeat);
					break;
				case StompCommand.Error:
					AppendHeader(sb, StompHeader.Message);
					break;
				case StompCommand.Message:
					AppendHeader(sb, StompHeader.MessageId);
					AppendHeader(sb, StompHeader.Subscription);
					break;
				case StompCommand.Receipt:
					AppendHeader(sb, StompHeader.ReceiptId);
					break;
				case StompCommand.Send:
					AppendHeader(sb, StompHeader.Destination);
					break;
				case StompCommand.Subscribe:
					AppendHeader(sb, StompHeader.Id);
					AppendHeader(sb, StompHeader.Destination);
					AppendHeaderIfPresent(sb, StompHeader.Ack);
					break;
				case StompCommand.Unsubscribe:
					AppendHeader(sb, StompHeader.Id);
					break;
			}

			AppendHeaderIfPresent(sb, StompHeader.Receipt);
			AppendContentLength(sb);
			AppendHeaderIfPresent(sb, StompHeader.NonStandard.ClrType);

			if (Command == StompCommand.Send || Command == StompCommand.Message)
			{
				AppendBodyIfText(sb);
			}

			return sb.ToString();
		}

		private void AppendHeader(StringBuilder sb, string header)
		{
			sb.Append(' ');
			sb.Append(header);
			sb.Append(':');
			sb.Append(Headers[header]);
		}

		private void AppendHeaderIfPresent(StringBuilder sb, string header)
		{
			var value = Headers[header];
			if (value != null)
			{
				sb.Append(' ');
				sb.Append(header);
				sb.Append(':');
				sb.Append(value);
			}
		}

		private void AppendContentLength(StringBuilder sb)
		{
			var value = Headers[StompHeader.ContentLength];
			if (value != null && value != "0")
			{
				sb.Append(' ');
				sb.Append(StompHeader.ContentLength);
				sb.Append(':');
				sb.Append(value);
			}
		}

		private void AppendBodyIfText(StringBuilder sb)
		{
			var contentType = Headers[StompHeader.ContentType];
			if (contentType.StartsWith("text/")
			    || contentType.StartsWith("application/xml")
			    || contentType.StartsWith("application/json"))
			{
				var whiteSpaceRegex = new Regex(@"\s+");
				var text = (BodyText ?? string.Empty);
				text = whiteSpaceRegex.Replace(text, " ");

				if (text.Length > 350)
				{
					text = text.Substring(0, 350) + "...";
				}

				sb.Append(' ');
				sb.Append(text.Trim());
			}
		}
	}
}