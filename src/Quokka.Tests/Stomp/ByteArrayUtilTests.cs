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

// ReSharper disable InconsistentNaming
namespace Quokka.Stomp
{
	[TestFixture]
	public class ByteArrayUtilTests
	{
		[Test]
		public void FindLengthToNull()
		{
			byte[] data = new byte[]
			              	{
			              		0x41,
			              		0x42,
			              		0x43,
			              		0x44,
			              		0,
			              		0x45,
			              		0x46,
			              		0,
			              	};

			Assert.AreEqual(4, ByteArrayUtil.FindLengthToNull(data, 0, data.Length));
			Assert.AreEqual(0, ByteArrayUtil.FindLengthToNull(data, 4, data.Length - 4));
		}

		[Test]
		public void Carriage_return_by_itself_is_not_a_new_line()
		{
			byte[] data = new byte[] { 0x41, 0x42, 0x0d, 0x0a, };

			Assert.AreEqual(-1, ByteArrayUtil.FindLineLength(data, 0, 3));
			Assert.AreEqual(2, ByteArrayUtil.FindLineLength(data, 0, 4));
		}

		[Test]
		public void LineHandling()
		{
			byte[] data = new byte[]
			              	{
			              		0x41,
			              		0x42,
			              		0x43,
			              		0x0d,
			              		0x0a,
			              		0x61,
			              		0x62,
			              		0x0d,
			              		0x0a,
			              		0x41,
			              		0x42,
			              		0x0a,
			              		0x43,
			              		0x44,
			              		0x0a,
			              		0x45,
			              		0x46,
			              	};

			int offset = 0;
			int length = data.Length;

			int lineLength = ByteArrayUtil.FindLineLength(data, offset, length);
			Assert.AreEqual(3, lineLength);
			offset += lineLength;
			length -= lineLength;
			Assert.IsTrue(ByteArrayUtil.SkipNewLine(data, ref offset, ref length));
			Assert.AreEqual(5, offset, "Unexpected offset");
			Assert.AreEqual(12, length, "Unexpected length");

			lineLength = ByteArrayUtil.FindLineLength(data, offset, length);
			Assert.AreEqual(2, lineLength);
			offset += lineLength;
			length -= lineLength;
			Assert.IsTrue(ByteArrayUtil.SkipNewLine(data, ref offset, ref length));
			Assert.AreEqual(9, offset, "Unexpected offset");
			Assert.AreEqual(8, length, "Unexpected length");

			lineLength = ByteArrayUtil.FindLineLength(data, offset, length);
			Assert.AreEqual(2, lineLength);
			offset += lineLength;
			length -= lineLength;
			Assert.IsTrue(ByteArrayUtil.SkipNewLine(data, ref offset, ref length));
			Assert.AreEqual(12, offset, "Unexpected offset");
			Assert.AreEqual(5, length, "Unexpected length");

			lineLength = ByteArrayUtil.FindLineLength(data, offset, length);
			Assert.AreEqual(2, lineLength);
			offset += lineLength;
			length -= lineLength;
			Assert.IsTrue(ByteArrayUtil.SkipNewLine(data, ref offset, ref length));
			Assert.AreEqual(15, offset, "Unexpected offset");
			Assert.AreEqual(2, length, "Unexpected length");

			lineLength = ByteArrayUtil.FindLineLength(data, offset, length);
			Assert.AreEqual(-1, lineLength);

			offset = data.Length;
			length = 0;
			Assert.IsFalse(ByteArrayUtil.SkipNewLine(data, ref offset, ref length));
			Assert.AreEqual(data.Length, offset);
			Assert.AreEqual(0, length);

			offset = 0;
			length = data.Length;
			Assert.IsFalse(ByteArrayUtil.SkipNewLine(data, ref offset, ref length));
		}
	}
}