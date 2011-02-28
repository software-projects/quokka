using System;
using NUnit.Framework;

namespace Quokka.Sandbox
{
	[TestFixture]
	public class ExceptionExtensionsTests
	{
		[Test]
		public void OutOfMemory()
		{
			var ex = new OutOfMemoryException();
			Assert.IsTrue(ex.IsCorruptedStateException());
		}

		[Test]
		public void AccessViolation()
		{
			var ex = new AccessViolationException();
			Assert.IsTrue(ex.IsCorruptedStateException());
		}

		[Test]
		public void StackOverflowException()
		{
			var ex = new StackOverflowException();
			Assert.IsTrue(ex.IsCorruptedStateException());
		}

		[Test]
		public void NotCorruptedStateExceptions()
		{
			var ex1 = new ArgumentNullException();
			Assert.IsFalse(ex1.IsCorruptedStateException());

			var ex2 = new Exception();
			Assert.IsFalse(ex2.IsCorruptedStateException());
		}
	}
}