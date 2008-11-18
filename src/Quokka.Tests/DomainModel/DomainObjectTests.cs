using NUnit.Framework;

namespace Quokka.DomainModel
{
	[TestFixture]
	public class DomainObjectTests
	{
		[Test]
		public void HashCodesAreBasedOnIdentifier()
		{
			SomeObj obj1 = new SomeObj(32);
			SomeObj obj2 = new SomeObj(32);

			Assert.AreEqual(obj1.GetHashCode(), obj2.GetHashCode());
			obj2.SetId(33);
			Assert.AreNotEqual(obj1.GetHashCode(), obj2.GetHashCode());
			obj1.SetId(33);
			Assert.AreEqual(obj1.GetHashCode(), obj2.GetHashCode());
		}

		[Test]
		public void UnsavedObjectsAreNotEqual()
		{
			// These two objects are not equal even if their Ids are equal (ie zero), because
			// zero is special and means not saved.
			SomeObj obj1 = new SomeObj();
			SomeObj obj2 = new SomeObj();
			Assert.AreNotEqual(obj1, obj2);

			// but they are equal to themselves
			Assert.AreEqual(obj1, obj1);
			Assert.AreEqual(obj2, obj2);
		}

		[Test]
		public void SavedObjectsCompareByIdentitity()
		{
			// simulate saved objects by setting non-zero identifier
			SomeObj obj1 = new SomeObj(42);
			SomeObj obj2 = new SomeObj(42);

			Assert.AreEqual(obj1, obj2);
		}

		private class SomeObj : DomainObject<SomeObj>
		{
			public SomeObj()
			{
			}

			public SomeObj(int id)
			{
				Id = id;
			}

			public void SetId(int id)
			{
				Id = id;
			}
		}
	}
}