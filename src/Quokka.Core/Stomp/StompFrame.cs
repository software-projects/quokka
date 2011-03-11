using System;
using System.IO;
using System.Text;
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
			if (Command == null)
			{
				throw new InvalidOperationException("Command not specified");
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
					var writer = new StreamWriter(stream, Encoding.UTF8);
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
	}
}