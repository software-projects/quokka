using System;
using NUnit.Framework;

namespace Quokka.NH
{
	[TestFixture]
	public class NHEntityTests
	{
		[Test]
		public void Int32Tests()
		{
			var id1A = new Int32Entity(1);
			var id1B = new Int32Entity(1);
			Assert.AreEqual(id1A, id1B);

			// default values mean entities are not equal
			var id0A = new Int32Entity(0);
			var id0B = new Int32Entity(0);
			Assert.AreNotEqual(id0A, id0B);

			var id2 = new Int32Entity(2);

			Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
			Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
			Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
		}

		[Test]
		public void Int64Tests()
		{
			var id1A = new Int64Entity(1);
			var id1B = new Int64Entity(1);
			Assert.AreEqual(id1A, id1B);

			// default values mean entities are not equal
			var id0A = new Int64Entity(0);
			var id0B = new Int64Entity(0);
			Assert.AreNotEqual(id0A, id0B);

			var id2 = new Int64Entity(2);

			Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
			Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
			Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
		}

		[Test]
		public void ValueTypeTests()
		{
			var id1A = new EnumEntity(EnumType.Enum1);
			var id1B = new EnumEntity(EnumType.Enum1);
			Assert.AreEqual(id1A, id1B);

			// default values mean entities are not equal
			var id0A = new EnumEntity(default(EnumType));
			var id0B = new EnumEntity(default(EnumType));
			Assert.AreNotEqual(id0A, id0B);

			var id2 = new EnumEntity(EnumType.Enum2);

			Assert.That(id0A.CompareTo(id1A), Is.LessThan(0));
			Assert.That(id1A.CompareTo(id2), Is.LessThan(0));
			Assert.That(id2.CompareTo(id1B), Is.GreaterThan(0));
		}

		[Test]
		public void ClassTypeTests()
		{
			var guid1 = Guid.NewGuid();
			var guid2 = Guid.NewGuid();

			// We want guid1 < guid2
			if (guid1.CompareTo(guid2) > 0)
			{
				var temp = guid1;
				guid1 = guid2;
				guid2 = temp;
			}
			Assert.IsTrue(guid1.CompareTo(guid2) < 0);

			var id1A = new GuidEntity(guid1);
			var id1B = new GuidEntity(guid1);
			Assert.AreEqual(id1A, id1B);

			// default values mean entities are not equal
			var id0A = new GuidEntity(default(Guid));
			var id0B = new GuidEntity(default(Guid));
			Assert.AreNotEqual(id0A, id0B);

			var id2 = new GuidEntity(guid2);

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

		public class Int32Entity : NHEntity<Int32Entity, int>
		{
			public readonly int Id;

			public Int32Entity(int id)
			{
				Id = id;
			}

			protected override int GetId()
			{
				return Id;
			}
		}

		public class Int64Entity : NHEntity<Int64Entity, long>
		{
			public readonly long Id;

			public Int64Entity(long id)
			{
				Id = id;
			}

			protected override long GetId()
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

		public enum EnumType
		{
			Enum1 = 1,
			Enum2 = 2,
		}

		public class EnumEntity : NHEntity<EnumEntity, EnumType>
		{
			public readonly EnumType Id;

			public EnumEntity(EnumType id)
			{
				Id = id;
			}

			protected override EnumType GetId()
			{
				return Id;
			}
		}

		public class GuidEntity : NHEntity<GuidEntity, Guid>
		{
			public readonly Guid Id;

			public GuidEntity(Guid id)
			{
				Id = id;
			}

			protected override Guid GetId()
			{
				return Id;
			}
		}
	}
}
