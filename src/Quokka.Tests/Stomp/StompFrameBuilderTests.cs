using System;
using System.IO;
using System.Text;
using NUnit.Framework;

// ReSharper disable InconsistentNaming

namespace Quokka.Stomp
{
	[TestFixture]
	public class StompFrameBuilderTests
	{
		public void SetUp()
		{
		}

		[Test]
		public void Receive_small_frame_in_one_receive()
		{
			const string payloadText = "<message>This is a message</message>";
			byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadText);
			MemoryStream memoryStream = new MemoryStream(payloadBytes);

			var originalFrame = new StompFrame
			                    	{
			                    		Command = "SEND",
			                    		Headers =
			                    			{
			                    				{"destination", "/queue/a"},
			                    				{"transaction", "tx-112"},
			                    			},
			                    		Body = memoryStream,
			                    	};

			byte[] data = originalFrame.ToArray();

			var builder = new StompFrameBuilder();
			builder.ReceiveBytes(data, 0, data.Length);

			Assert.IsTrue(builder.IsFrameReady, "Unexpected result");

			var receivedFrame = builder.GetNextFrame();
			Assert.IsNotNull(receivedFrame);
			Assert.IsNull(builder.GetNextFrame());

			Assert.AreEqual(originalFrame.Command, receivedFrame.Command);
			Assert.AreEqual(originalFrame.Headers["destination"], receivedFrame.Headers["destination"]);
			Assert.AreEqual(originalFrame.Headers["transaction"], receivedFrame.Headers["transaction"]);
			Assert.AreEqual(originalFrame.Headers["content-length"], receivedFrame.Headers["content-length"]);
			Assert.AreEqual(originalFrame.Headers.Count, receivedFrame.Headers.Count);
			Assert.AreEqual(payloadBytes.Length, receivedFrame.Body.Length);

			string receivedText = Encoding.UTF8.GetString(receivedFrame.Body.ToArray());
			Assert.AreEqual(payloadText, receivedText);
		}

		[Test]
		public void Receive_two_frames_in_one_receive()
		{
			const string payloadText = "<message>This is a message</message>";
			byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadText);
			MemoryStream memoryStream = new MemoryStream(payloadBytes);

			var originalFrame = new StompFrame
			                    	{
			                    		Command = "SEND",
			                    		Headers =
			                    			{
			                    				{"destination", "/queue/a"},
			                    				{"transaction", "tx-112"},
			                    			},
			                    		Body = memoryStream,
			                    	};

			byte[] data1 = originalFrame.ToArray();

			originalFrame.Headers["destination"] = "/queue/b";
			originalFrame.Headers["transaction"] = "tx-113";

			byte[] data2 = originalFrame.ToArray();

			byte[] combinedData = new byte[data1.Length + data2.Length];
			Array.Copy(data1, combinedData, data1.Length);
			Array.Copy(data2, 0, combinedData, data1.Length, data2.Length);

			var builder = new StompFrameBuilder();

			builder.ReceiveBytes(combinedData, 0, combinedData.Length);
			Assert.IsTrue(builder.IsFrameReady);

			var receivedFrame1 = builder.GetNextFrame();
			Assert.IsNotNull(receivedFrame1);
			var receivedFrame2 = builder.GetNextFrame();
			Assert.IsNotNull(receivedFrame2);
			Assert.IsNull(builder.GetNextFrame());

			Assert.AreEqual(originalFrame.Command, receivedFrame1.Command);
			Assert.AreEqual("/queue/a", receivedFrame1.Headers["destination"]);
			Assert.AreEqual("tx-112", receivedFrame1.Headers["transaction"]);
			Assert.AreEqual(originalFrame.Headers.Count, receivedFrame1.Headers.Count);
			Assert.AreEqual(payloadBytes.Length, receivedFrame1.Body.Length);
			Assert.AreEqual(payloadText, Encoding.UTF8.GetString(receivedFrame1.Body.ToArray()));

			Assert.AreEqual(originalFrame.Command, receivedFrame2.Command);
			Assert.AreEqual("/queue/b", receivedFrame2.Headers["destination"]);
			Assert.AreEqual("tx-113", receivedFrame2.Headers["transaction"]);
			Assert.AreEqual(originalFrame.Headers.Count, receivedFrame2.Headers.Count);
			Assert.AreEqual(payloadBytes.Length, receivedFrame2.Body.Length);
			Assert.AreEqual(payloadText, Encoding.UTF8.GetString(receivedFrame2.Body.ToArray()));
		}

		[Test]
		public void Receive_two_frames_in_one_byte_chunks()
		{
			const string payloadText = "<message>This is a message</message>";
			byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadText);
			MemoryStream memoryStream = new MemoryStream(payloadBytes);

			var originalFrame = new StompFrame
			                    	{
			                    		Command = "SEND",
			                    		Headers =
			                    			{
			                    				{"destination", "/queue/a"},
			                    				{"transaction", "tx-112"},
			                    			},
			                    		Body = memoryStream,
			                    	};

			byte[] data1 = originalFrame.ToArray();

			originalFrame.Headers["destination"] = "/queue/b";
			originalFrame.Headers["transaction"] = "tx-113";

			byte[] data2 = originalFrame.ToArray();

			byte[] combinedData = new byte[data1.Length + data2.Length];
			Array.Copy(data1, combinedData, data1.Length);
			Array.Copy(data2, 0, combinedData, data1.Length, data2.Length);

			var builder = new StompFrameBuilder();

			foreach (byte @byte in combinedData)
			{
				var array = new[] {@byte};
				builder.ReceiveBytes(array, 0, 1);
			}

			Assert.IsTrue(builder.IsFrameReady);

			var receivedFrame1 = builder.GetNextFrame();
			Assert.IsNotNull(receivedFrame1);
			var receivedFrame2 = builder.GetNextFrame();
			Assert.IsNotNull(receivedFrame2);
			Assert.IsNull(builder.GetNextFrame());

			Assert.AreEqual(originalFrame.Command, receivedFrame1.Command);
			Assert.AreEqual("/queue/a", receivedFrame1.Headers["destination"]);
			Assert.AreEqual("tx-112", receivedFrame1.Headers["transaction"]);
			Assert.AreEqual(originalFrame.Headers.Count, receivedFrame1.Headers.Count);
			Assert.AreEqual(payloadBytes.Length, receivedFrame1.Body.Length);
			Assert.AreEqual(payloadText, Encoding.UTF8.GetString(receivedFrame1.Body.ToArray()));

			Assert.AreEqual(originalFrame.Command, receivedFrame2.Command);
			Assert.AreEqual("/queue/b", receivedFrame2.Headers["destination"]);
			Assert.AreEqual("tx-113", receivedFrame2.Headers["transaction"]);
			Assert.AreEqual(originalFrame.Headers.Count, receivedFrame2.Headers.Count);
			Assert.AreEqual(payloadBytes.Length, receivedFrame2.Body.Length);
			Assert.AreEqual(payloadText, Encoding.UTF8.GetString(receivedFrame2.Body.ToArray()));
		}

		[Test]
		public void Receive_two_frames_in_one_receive_with_content_length_and_no_separating_null()
		{
			const string payloadText = "<message>This is a message</message>";
			byte[] payloadBytes = Encoding.UTF8.GetBytes(payloadText);
			MemoryStream memoryStream = new MemoryStream(payloadBytes);

			var originalFrame = new StompFrame
			                    	{
			                    		Command = "SEND",
			                    		Headers =
			                    			{
			                    				{"destination", "/queue/a"},
			                    				{"transaction", "tx-112"},
			                    			},
			                    		Body = memoryStream,
			                    	};

			byte[] data1 = originalFrame.ToArray();

			originalFrame.Headers["destination"] = "/queue/b";
			originalFrame.Headers["transaction"] = "tx-113";

			byte[] data2 = originalFrame.ToArray();


			// by copying one byte less, we lose the null byte
			byte[] combinedData = new byte[data1.Length + data2.Length - 2];
			Array.Copy(data1, combinedData, data1.Length - 1);
			Array.Copy(data2, 0, combinedData, data1.Length - 1, data2.Length - 1);

			var builder = new StompFrameBuilder();

			builder.ReceiveBytes(combinedData, 0, combinedData.Length);
			Assert.IsTrue(builder.IsFrameReady);

			var receivedFrame1 = builder.GetNextFrame();
			Assert.IsNotNull(receivedFrame1);
			var receivedFrame2 = builder.GetNextFrame();
			Assert.IsNotNull(receivedFrame2);
			Assert.IsNull(builder.GetNextFrame());

			Assert.AreEqual(originalFrame.Command, receivedFrame1.Command);
			Assert.AreEqual("/queue/a", receivedFrame1.Headers["destination"]);
			Assert.AreEqual("tx-112", receivedFrame1.Headers["transaction"]);
			Assert.AreEqual(originalFrame.Headers.Count, receivedFrame1.Headers.Count);
			Assert.AreEqual(payloadBytes.Length, receivedFrame1.Body.Length);
			Assert.AreEqual(payloadText, Encoding.UTF8.GetString(receivedFrame1.Body.ToArray()));

			Assert.AreEqual(originalFrame.Command, receivedFrame2.Command);
			Assert.AreEqual("/queue/b", receivedFrame2.Headers["destination"]);
			Assert.AreEqual("tx-113", receivedFrame2.Headers["transaction"]);
			Assert.AreEqual(originalFrame.Headers.Count, receivedFrame2.Headers.Count);
			Assert.AreEqual(payloadBytes.Length, receivedFrame2.Body.Length);
			Assert.AreEqual(payloadText, Encoding.UTF8.GetString(receivedFrame2.Body.ToArray()));
		}

		[Test]
		public void Header_value_contains_colon()
		{
			var originalFrame = new StompFrame
			                    	{
			                    		Command = "SEND",
			                    		Headers =
			                    			{
			                    				{"destination", "queue://this-contains:three:colons"},
			                    			},
			                    	};

			byte[] data = originalFrame.ToArray();
			var builder = new StompFrameBuilder();
			builder.ReceiveBytes(data, 0, data.Length);
			var receivedFrame = builder.GetNextFrame();
			Assert.IsNotNull(receivedFrame);
			Assert.AreEqual(originalFrame.Headers["destination"], receivedFrame.Headers["destination"]);
			Assert.AreEqual("queue://this-contains:three:colons", receivedFrame.Headers["destination"]);
		}
	}
}