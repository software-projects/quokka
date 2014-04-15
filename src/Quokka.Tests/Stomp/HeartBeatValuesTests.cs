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
