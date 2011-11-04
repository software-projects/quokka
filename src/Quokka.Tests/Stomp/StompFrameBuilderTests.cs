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

			var builder = new StompFrameBuilder();
			var segment = builder.ToArray(originalFrame);

			builder.ReceiveBytes(segment.Array, segment.Offset, segment.Count);

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

			var builder = new StompFrameBuilder();

			var data1 = builder.ToArray(originalFrame);

			originalFrame.Headers["destination"] = "/queue/b";
			originalFrame.Headers["transaction"] = "tx-113";

			var data2 = builder.ToArray(originalFrame);

			byte[] combinedData = new byte[data1.Count + data2.Count];
			Array.Copy(data1.Array, data1.Offset, combinedData, 0, data1.Count);
			Array.Copy(data2.Array, data2.Offset, combinedData, data1.Count, data2.Count);

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

			var builder = new StompFrameBuilder();

			var data1 = builder.ToArray(originalFrame);

			originalFrame.Headers["destination"] = "/queue/b";
			originalFrame.Headers["transaction"] = "tx-113";

			var data2 = builder.ToArray(originalFrame);

			byte[] combinedData = new byte[data1.Count + data2.Count];
			Array.Copy(data1.Array, data1.Offset, combinedData, 0, data1.Count);
			Array.Copy(data2.Array, data2.Offset, combinedData, data1.Count, data2.Count);

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

			var builder = new StompFrameBuilder();

			var data1 = builder.ToArray(originalFrame);

			originalFrame.Headers["destination"] = "/queue/b";
			originalFrame.Headers["transaction"] = "tx-113";

			var data2 = builder.ToArray(originalFrame);


			// by copying one byte less, we lose the null byte
			byte[] combinedData = new byte[data1.Count + data2.Count - 2];
			Array.Copy(data1.Array, data1.Offset, combinedData, 0, data1.Count - 1);
			Array.Copy(data2.Array, data2.Offset, combinedData, data1.Count - 1, data2.Count - 1);

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

			var builder = new StompFrameBuilder();
			var data = builder.ToArray(originalFrame);
			builder.ReceiveBytes(data.Array, data.Offset, data.Count);
			var receivedFrame = builder.GetNextFrame();
			Assert.IsNotNull(receivedFrame);
			Assert.AreEqual(originalFrame.Headers["destination"], receivedFrame.Headers["destination"]);
			Assert.AreEqual("queue://this-contains:three:colons", receivedFrame.Headers["destination"]);
		}

		[Test]
		public void Serialize_keep_alive()
		{
			var builder = new StompFrameBuilder();

			var data = builder.ToArray(null);
			Assert.AreEqual(1, data.Count);
			Assert.AreEqual(10, data.Array[data.Offset]);

			data = builder.ToArray(StompFrame.HeartBeat);
			Assert.AreEqual(1, data.Count);
			Assert.AreEqual(10, data.Array[data.Offset]);

			data = builder.ToArray(new StompFrame(string.Empty));
			Assert.AreEqual(1, data.Count);
			Assert.AreEqual(10, data.Array[data.Offset]);

			Assert.IsTrue(StompFrame.HeartBeat.IsHeartBeat);
		}
	}
}