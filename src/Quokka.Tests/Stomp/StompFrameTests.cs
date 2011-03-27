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
		public void Missing_header_results_in_null_value()
		{
			var message = new StompFrame();
			Assert.IsNull(message.Headers["missing-header"]);
		}

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

			const string expectedText = "CONNECT\r\nlogin:scott\r\npasscode:tiger\r\ncontent-length:0\r\n\r\n\0";
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
			                      + "content-type:application/xml; encoding=UTF8\r\n"
			                      + "content-length:" + payload.Length + "\r\n\r\n"
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
		public void Null_command_results_in_heartbeat()
		{
			var data = new StompFrame { Command = null }.ToArray();
			Assert.AreEqual(1, data.Length);
			Assert.AreEqual(10, data[0]);
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

		[Test]
		public void Serialize()
		{
			var s1 = new SerializeTestClass
			         	{
			         		Number = 123,
			         		Text = "This is some text"
			         	};

			var frame = new StompFrame();
			frame.Serialize(s1);

			var s2 = (SerializeTestClass)frame.Deserialize();
			Assert.IsNotNull(s2);
			Assert.AreEqual(s1.Number, s2.Number);
			Assert.AreEqual(s1.Text, s2.Text);
		}

		[Test]
		public void Cannot_deserialize_without_content_type()
		{
			var s1 = new SerializeTestClass
			{
				Number = 123,
				Text = "This is some text"
			};

			var frame = new StompFrame();
			frame.Serialize(s1);
			frame.Headers[StompHeader.ContentType] = "text/plain";

			try
			{
				frame.Deserialize();
				Assert.Fail("Expected exception");
			}
			catch (InvalidOperationException ex)
			{
				Assert.AreEqual("Cannot deserialize: content-type:text/plain", ex.Message);
			}
		}

		[Test]
		public void Cannot_deserialize_without_clr_type()
		{
			var s1 = new SerializeTestClass
			{
				Number = 123,
				Text = "This is some text"
			};

			var frame = new StompFrame();
			frame.Serialize(s1);
			frame.Headers[StompHeader.NonStandard.ClrType] = null;

			try
			{
				frame.Deserialize();
				Assert.Fail("Expected exception");
			}
			catch (InvalidOperationException ex)
			{
				Assert.AreEqual("Cannot deserialize: no clr-type specified", ex.Message);
			}
		}

		public class SerializeTestClass
		{
			public int Number;
			public string Text;
		}

		[Test]
		public void Expires_header()
		{
			var frame = new StompFrame();
			var dateTime = new DateTimeOffset(2099, 11, 10, 19, 18, 17, 16, TimeSpan.FromHours(10));

			frame.SetExpires(dateTime);
			Assert.AreEqual("20991110T091817Z", frame.Headers[StompHeader.NonStandard.Expires]);

			// truncated milliseconds means not expired
			Assert.IsFalse(frame.IsExpiredAt(dateTime - TimeSpan.FromSeconds(1)));

			Assert.IsTrue(frame.IsExpiredAt(dateTime + TimeSpan.FromSeconds(1)));
		}

		[Test]
		public void KeepAlive_frame()
		{
			var frame = new StompFrame(string.Empty);
			Assert.IsTrue(frame.IsHeartBeat);
			frame.Command = null;
			Assert.IsTrue(frame.IsHeartBeat);
			frame.Command = "XXX";
			Assert.IsFalse(frame.IsHeartBeat);
		}
	}
}