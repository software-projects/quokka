using System;
using System.Reflection;

using NUnit.Framework;

namespace Quokka.Reflection
{
    [TestFixture]
    public class TypeUtilTests
    {
        [Test]
        public void FindType_NoNamespaces() {
            Assembly assembly = typeof(QuokkaContainer).Assembly;
            Type type = TypeUtil.FindType("quokkacontainer", null, assembly);
            Assert.IsNull(type);

            type = TypeUtil.FindType("Quokka.quokkacontainer", null, assembly);
            Assert.AreEqual(typeof(QuokkaContainer), type);
        }

        [Test]
        public void FindType_Namespace() {
            Assembly assembly = typeof(QuokkaContainer).Assembly;
            string[] namespaces = new string[] { "Quokka", "Quokka.Uip" };

            Type type = TypeUtil.FindType("Quokkacontainer", namespaces, assembly);
            Assert.AreEqual(typeof(QuokkaContainer), type);

            type = TypeUtil.FindType("UipNode", namespaces, assembly);
            Assert.AreEqual(typeof(Quokka.Uip.UipNode), type);

            type = TypeUtil.FindType("UipNode3", namespaces, assembly);
            Assert.IsNull(type);
        }

        [Test]
        public void FindType_Assemblies() {
            Assembly[] assemblies = new Assembly[] { typeof(QuokkaContainer).Assembly, Assembly.GetExecutingAssembly() };
            string[] namespaces = new string[] { "Quokka", "Quokka.Reflection" };

            Type type = TypeUtil.FindType("Quokkacontainer", namespaces, assemblies);
            Assert.AreEqual(typeof(QuokkaContainer), type);

            type = TypeUtil.FindType("PropertyUtilTests", namespaces, assemblies);
            Assert.AreEqual(typeof(Quokka.Reflection.PropertyUtilTests), type);

            type = TypeUtil.FindType("UipNode3", namespaces, assemblies);
            Assert.IsNull(type);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindType_ArgumentNull_1() {
            Type type = TypeUtil.FindType(null, null, Assembly.GetExecutingAssembly());
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindType_ArgumentNull_3a() {
            Assembly assembly = null;
            Type type = TypeUtil.FindType("XYZ", null, assembly);
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void FindType_ArgumentNull_3b() {
            Assembly[] assemblies = null;
            Type type = TypeUtil.FindType("XYZ", null, assemblies);
        }
    }
}
