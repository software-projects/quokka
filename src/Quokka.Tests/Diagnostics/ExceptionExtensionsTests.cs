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