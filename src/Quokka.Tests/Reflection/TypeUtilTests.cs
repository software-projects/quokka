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
using System.Reflection;

using NUnit.Framework;

// ReSharper disable InconsistentNaming
namespace Quokka.Reflection
{
    [TestFixture]
    public class TypeUtilTests
    {
        [Test]
        public void FindType_NoNamespaces() {
            Assembly assembly = typeof(QuokkaException).Assembly;
            Type type = TypeUtil.FindType("quokkacontainer", null, assembly);
            Assert.IsNull(type);

            type = TypeUtil.FindType("Quokka.quokkaexception", null, assembly);
			Assert.AreEqual(typeof(QuokkaException), type);
        }

        [Test]
        public void FindType_Namespace() {
			Assembly assembly = typeof(QuokkaException).Assembly;
            string[] namespaces = new string[] { "Quokka", "Quokka.UI.Tasks" };

            Type type = TypeUtil.FindType("Quokkaexception", namespaces, assembly);
			Assert.AreEqual(typeof(QuokkaException), type);

            type = TypeUtil.FindType("UINode", namespaces, assembly);
            Assert.AreEqual(typeof(Quokka.UI.Tasks.UINode), type);

            type = TypeUtil.FindType("UipNode3", namespaces, assembly);
            Assert.IsNull(type);
        }

        [Test]
        public void FindType_Assemblies() {
			Assembly[] assemblies = new Assembly[] { typeof(QuokkaException).Assembly, Assembly.GetExecutingAssembly() };
            string[] namespaces = new string[] { "Quokka", "Quokka.Reflection" };

            Type type = TypeUtil.FindType("Quokkaexception", namespaces, assemblies);
			Assert.AreEqual(typeof(QuokkaException), type);

            type = TypeUtil.FindType("PropertyUtilTests", namespaces, assemblies);
            Assert.AreEqual(typeof(Quokka.Reflection.PropertyUtilTests), type);

            type = TypeUtil.FindType("UipNode3", namespaces, assemblies);
            Assert.IsNull(type);
        }

        [Test]
        public void FindType_ArgumentNull_1() {
            Assert.Throws<ArgumentNullException>(() => TypeUtil.FindType(null, null, Assembly.GetExecutingAssembly()));
        }

        [Test]
        public void FindType_ArgumentNull_3a() {
            Assembly assembly = null;
            Assert.Throws<ArgumentNullException>(() => TypeUtil.FindType("XYZ", null, assembly));
        }

        [Test]
        public void FindType_ArgumentNull_3b() {
            Assembly[] assemblies = null;
            Assert.Throws<ArgumentNullException>(() => TypeUtil.FindType("XYZ", null, assemblies));
        }

		private class TestClass1
		{
			public interface ISomeInterface {}
		}

		private class TestClass2 : TestClass1 {}

		[Test]
		public void FindNestedInterface()
		{
			Assert.AreEqual(typeof (TestClass1.ISomeInterface),
			                TypeUtil.FindNestedInterface(typeof (TestClass1), "ISomeInterface"));

			Assert.AreEqual(typeof(TestClass1.ISomeInterface),
				TypeUtil.FindNestedInterface(typeof(TestClass2), "ISomeInterface"));

			Assert.IsNull(TypeUtil.FindNestedInterface(typeof (TestClass2), "ISomeOtherInterface"));
		}
    }
}
