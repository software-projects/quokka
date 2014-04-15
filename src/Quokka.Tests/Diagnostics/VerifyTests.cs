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