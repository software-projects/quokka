using NUnit.Framework;

namespace Quokka.NH
{
	[TestFixture]
	public class NHEntityTests
	{
		[Test]
		public void Int32Tests()
		{
			var id1A = new IntEntity(1);
			var id1B = new IntEntity(1);
			Assert.AreEqual(id1A, id1B);

			// default values mean entities are not equal
			var id0A = new IntEntity(0);
			var id0B = new IntEntity(0);
			Assert.AreNotEqual(id0A, id0B);

			var id2 = new IntEntity(2);

			Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
			Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
			Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
		}

		[Test]
		public void StringTests()
		{
			var id1A = new StringEntity("XX");
			var id1B = new StringEntity("xx");
			Assert.AreEqual(id1A, id1B);

			// default values mean entities are not equal
			var id0A = new StringEntity(null);
			var id0B = new StringEntity(null);
			Assert.AreNotEqual(id0A, id0B);

			var id2 = new StringEntity("ZZ");

			Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
			Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
			Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
		}

		public class IntEntity : NHEntity<IntEntity, int>
		{
			public readonly int Id;

			public IntEntity(int id)
			{
				Id = id;
			}

			protected override int GetId()
			{
				return Id;
			}
		}

		public class StringEntity : NHEntity<StringEntity, string>
		{
			public readonly string Id;

			public StringEntity(string id)
			{
				Id = id;
			}

			protected override string GetId()
			{
				return Id;
			}
		}
	}
}
