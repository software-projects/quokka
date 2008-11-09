using System;
using NUnit.Framework;

namespace Quokka.Diagnostics
{
	[TestFixture]
	public class VerifyTests
	{
		private class TestObject
		{
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void ArgumentNullThrowsArgumentNullException()
		{
			object obj = null;
			Verify.ArgumentNotNull(obj, "paramName");
		}

		[Test]
		public void CopiesValueWhenNotNull()
		{
			TestObject obj1 = new TestObject();
			TestObject obj2;

			Verify.ArgumentNotNull(obj1, "paramName", out obj2);
			Assert.IsNotNull(obj2);
			Assert.AreSame(obj1, obj2);
		}

		[Test]
		public void DoesNotThrowWhenNotNull()
		{
			object obj = new object();
			Verify.ArgumentNotNull(obj, "paramName");
		}

		[Test]
		[ExpectedException(typeof (ArgumentNullException))]
		public void GenericVersionThrowsExceptionWhenNull()
		{
			TestObject obj1 = null;
			TestObject obj2;

			Verify.ArgumentNotNull(obj1, "paramName", out obj2);
		}
	}
}