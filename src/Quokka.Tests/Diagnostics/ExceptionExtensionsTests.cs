using System;
using NUnit.Framework;

namespace Quokka.Diagnostics
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
			// this used to be considered a corrupted state exception but is not anymore
			var ex = new StackOverflowException();
			Assert.IsFalse(ex.IsCorruptedStateException());
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