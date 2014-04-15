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
using System.Globalization;
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
		public void Headers_cannot_be_set_to_null()
		{
			Assert.Throws<ArgumentNullException>(() => new StompFrame {Headers = null});
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
		
		[Test]
		public void ToString_with_xml_body()
		{
			var frame = new StompFrame
			{
				Command = "MESSAGE",
				Headers = new StompHeaderCollection
			            		          	{
			            		          		{"subscription", "1"},
			            		          		{"content-length", "123"},
												{"content-type", "application/xml"},
												{"clr-type", "Test.Type,Test"},
												{"message-id", "6"},
			            		          	},
				Body = new MemoryStream(),
			};

			var writer = new StreamWriter(frame.Body);
			writer.WriteLine("<?xml version='1.0'>");
			writer.WriteLine("<Test>");
			writer.WriteLine("    <Name>This is the name</Name>");
			writer.WriteLine("</Test>");
			writer.Flush();

			frame.Headers["content-length"] = frame.Body.Length.ToString(CultureInfo.InvariantCulture);

			var actual = frame.ToString();
			const string expected = "MESSAGE message-id:6 subscription:1 content-length:74 clr-type:Test.Type,Test "
			                        + "<?xml version='1.0'> <Test> <Name>This is the name</Name> </Test>";
			Assert.AreEqual(expected, actual);
		}

		[Test]
		public void ToString_with_large_text_body()
		{
			var frame = new StompFrame
			{
				Command = "MESSAGE",
				Headers = new StompHeaderCollection
			            		          	{
			            		          		{"subscription", "1"},
			            		          		{"content-length", "123"},
												{"content-type", "text/plain"},
												{"message-id", "6"},
			            		          	},
				Body = new MemoryStream(),
			};

			var writer = new StreamWriter(frame.Body);
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.WriteLine("12345678901234567890123456789012345678901234567890");
			writer.Flush();

			frame.Headers["content-length"] = frame.Body.Length.ToString(CultureInfo.InvariantCulture);

			var actual = frame.ToString();
			const string expected = "MESSAGE message-id:6 subscription:1 content-length:520 "
			                        + "12345678901234567890123456789012345678901234567890 "
			                        + "12345678901234567890123456789012345678901234567890 "
									+ "12345678901234567890123456789012345678901234567890 "
									+ "12345678901234567890123456789012345678901234567890 "
									+ "12345678901234567890123456789012345678901234567890 "
									+ "12345678901234567890123456789012345678901234567890 "
									+ "12345678901234567890123456789012345678901234...";
			Assert.AreEqual(expected, actual);
		}
	}
}