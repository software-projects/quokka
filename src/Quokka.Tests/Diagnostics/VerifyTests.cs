﻿using System;
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
		public void ArgumentNullThrowsArgumentNullException()
		{
			object obj = null;
			Assert.Throws<ArgumentNullException>(() => Verify.ArgumentNotNull(obj, "paramName"));
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
		public void GenericVersionThrowsExceptionWhenNull()
		{
			TestObject obj1 = null;
			TestObject obj2;

			Assert.Throws<ArgumentNullException>(() => Verify.ArgumentNotNull(obj1, "paramName", out obj2));
		}
	}
}