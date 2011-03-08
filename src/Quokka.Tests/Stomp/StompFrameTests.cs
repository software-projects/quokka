using System;
using System.IO;
using System.Text;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Quokka.Stomp
{
	[TestFixture]
	public class StompFrameTests
	{
		[Test]
		public void ToArray_with_empty_body()
		{
			var frame = new StompFrame
			            	{
			            		Command = "CONNECT",
			            		Headers = new StompHeaderCollection
			            		          	{
			            		          		{"login", "scott"},
			            		          		{"passcode", "tiger"},
			            		          	},
			            		Body = null,
			            	};

			var bytes = frame.ToArray();

			var text = new StreamReader(new MemoryStream(bytes), Encoding.UTF8).ReadToEnd();

			const string expectedText = "CONNECT\r\nlogin: scott\r\npasscode: tiger\r\ncontent-length: 0\r\n\r\n\0";
			Assert.AreEqual(expectedText, text);
		}


		[Test]
		public void ToArray_with_xml_body()
		{
			const string someXml = "<message>hello world</message>";
			byte[] payload = Encoding.UTF8.GetBytes(someXml);
			MemoryStream body = new MemoryStream(payload);

			var frame = new StompFrame
			            	{
			            		Command = "SEND",
			            		Headers = new StompHeaderCollection
			            		          	{
			            		          		{"content-type", "application/xml; encoding=UTF8"},
			            		          	},
			            		Body = body,
			            	};

			var bytes = frame.ToArray();

			var text = new StreamReader(new MemoryStream(bytes), Encoding.UTF8).ReadToEnd();

			string expectedText = "SEND\r\n"
			                      + "content-type: application/xml; encoding=UTF8\r\n"
			                      + "content-length: " + payload.Length + "\r\n\r\n"
			                      + someXml
			                      + "\0";
			Assert.AreEqual(expectedText, text);
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void Headers_cannot_be_set_to_null()
		{
			new StompFrame {Headers = null};
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void Command_cannot_be_null()
		{
			new StompFrame().ToArray();
		}

		[Test]
		public void BodyText_tests()
		{
			var frame = new StompFrame
			            	{
			            		Command = "COMMAND",
			            		BodyText = "1",
			            	};

			Assert.AreEqual("1", frame.BodyText);
		}
	}
}