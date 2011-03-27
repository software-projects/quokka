using NUnit.Framework;
using Quokka.Stomp.Internal;

// ReSharper disable InconsistentNaming

namespace Quokka.Stomp
{
	[TestFixture]
	public class HeartBeatValuesTests
	{
		[Test]
		public void Parse_from_text()
		{
			var hb = new HeartBeatValues("30,20");
			Assert.AreEqual(30, hb.Outgoing);
			Assert.AreEqual(20, hb.Incoming);

			hb = new HeartBeatValues(null);
			Assert.AreEqual(0, hb.Outgoing);
			Assert.AreEqual(0, hb.Incoming);

			hb = new HeartBeatValues("   ");
			Assert.AreEqual(0, hb.Outgoing);
			Assert.AreEqual(0, hb.Incoming);

			hb = new HeartBeatValues("11");
			Assert.AreEqual(11, hb.Outgoing);
			Assert.AreEqual(0, hb.Incoming);

			hb = new HeartBeatValues(",11");
			Assert.AreEqual(0, hb.Outgoing);
			Assert.AreEqual(11, hb.Incoming);
		}

		[Test]
		public void To_string()
		{
			var hb = new HeartBeatValues("30,20");
			Assert.AreEqual("30,20", hb.ToString());
		}

		[Test]
		public void Combine_zero_outgoing()
		{
			var hb1 = new HeartBeatValues("0,5");
			var hb2 = new HeartBeatValues("10,5");

			var hb3 = hb1.CombineWith(hb2);
			Assert.AreEqual(0, hb3.Outgoing);
			Assert.AreEqual(10, hb3.Incoming);
		}

		[Test]
		public void Combine_zero_incoming()
		{
			var hb1 = new HeartBeatValues("10,0");
			var hb2 = new HeartBeatValues("10,5");

			var hb3 = hb1.CombineWith(hb2);
			Assert.AreEqual(10, hb3.Outgoing);
			Assert.AreEqual(0, hb3.Incoming);
		}
	}
}
