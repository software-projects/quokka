using System;
using System.Collections.Generic;
using NUnit.Framework;

using Quokka.DynamicCodeGeneration;

namespace Quokka.Tests
{
    [TestFixture]
    [Ignore("Not finished")]
    public class MemberMappingTests
    {
        public interface ISample
        {
            bool CanDoSomething { get; } // this will return true for Sample1
            bool CanDoAnotherThing { get; } // this will return false for Sample1
            bool IsMyPropertySupported(); // this is an alternative naming convention good for nouns
            bool IsAnotherPropertySupported { get; } // both properties and methods are supported

            void DoSomething();
            void DoAnotherThing();

            int MyProperty { get; }

            void MandatoryMethod();
        }

        public class Sample1
        {
            public void DoSomething() {}
            public void MandatoryMethod() { }
        }

        public class Sample2
        {
            public void DoSomething() { }
        }

        public class TestClass1 { }
        public interface ITestInterface1 { }

        [Test]
        public void ExpectInterfaceType() {
            Assert.Throws<QuokkaException>(() => new MemberMapping(typeof(TestClass1), typeof(Sample1)));
        }

        [Test]
        public void ExpectConcreteType() {
            Assert.Throws<QuokkaException>(() => new MemberMapping(typeof(ISample), typeof(ITestInterface1)));
        }

        [Test]
        public void ValidMapping() {
            MemberMapping mm = new MemberMapping(typeof(ISample), typeof(Sample1));
            Assert.IsTrue(mm.IsValid);
            Assert.IsTrue(String.IsNullOrEmpty(mm.ErrorMessage));
        }

        [Test]
        public void InvalidMapping() {
            MemberMapping mm = new MemberMapping(typeof(ISample), typeof(Sample2));
            Assert.IsFalse(mm.IsValid);
            Assert.IsFalse(String.IsNullOrEmpty(mm.ErrorMessage));
            Assert.IsTrue(mm.MissingMembers.Count > 0);
        }
    }
}
