

namespace Quokka
{
    using System;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceContainerTests
    {
        [Test]
        public void BasicTest() {
            QuokkaServiceContainer container = new QuokkaServiceContainer();

            container.AddService(typeof(IInterface1), typeof(Class1));
            container.AddService(typeof(IInterface2), typeof(Class2));

            IInterface2 i2 = (IInterface2)container.GetService(typeof(IInterface2));
            Assert.IsNotNull(i2);

            IInterface1 i1 = (IInterface1)container.GetService(typeof(IInterface1));
            Assert.IsNotNull(i1);
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Constructor for Quokka.ServiceContainerTests+Class1_StringParameter has a non-interface parameter: System.String connectionString")]
        public void StringParameterInConstructor() {
            QuokkaServiceContainer container = new QuokkaServiceContainer();
            container.AddService(typeof(IInterface1), typeof(Class1_StringParameter));
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage="Too many constructors for Quokka.ServiceContainerTests+Class1_MultipleConstructors")]
        public void MultipleConstructors() {
            QuokkaServiceContainer container = new QuokkaServiceContainer();
            container.AddService(typeof(IInterface1), typeof(Class1_MultipleConstructors));
        }

        [Test]
        public void GenericClass() {
            QuokkaServiceContainer container = new QuokkaServiceContainer();
            container.AddService(typeof(IInterface1), typeof(Class1_Generic<IInterface2>));
        }

        [Test, ExpectedException(typeof(QuokkaException), ExpectedMessage="Circular reference detected for Quokka.ServiceContainerTests+IInterface1")]
        public void CircularReference() {
            QuokkaServiceContainer container = new QuokkaServiceContainer();
            container.AddService(typeof(IInterface1), typeof(Class1_DependsOnIInterface3));
            container.AddService(typeof(IInterface2), typeof(Class2));
            container.AddService(typeof(IInterface3), typeof(Class3));

            IInterface1 i1 = (IInterface1)container.GetService(typeof(IInterface1));
        }

        [Test, ExpectedException(typeof(QuokkaException), ExpectedMessage = "No available implementation of Quokka.ServiceContainerTests+IInterface1")]
        public void MissingType() {
            QuokkaServiceContainer container = new QuokkaServiceContainer();
            container.AddService(typeof(IInterface2), typeof(Class2));
            container.AddService(typeof(IInterface3), typeof(Class3));

            IInterface2 i2 = (IInterface2)container.GetService(typeof(IInterface2));
        }

        [Test]
        public void ServiceProviderInConstructor() {
            QuokkaServiceContainer container = new QuokkaServiceContainer();
            container.AddService(typeof(IInterface1), typeof(Class1_ServiceProviderConstructor));

            IInterface1 i1 = (IInterface1)container.GetService(typeof(IInterface1));

        }

        #region Test interfaces

        public interface IInterface1
        {
            void DoSomething1();
        }

        public interface IInterface2
        {
            void DoSomething2();
        }

        public interface IInterface3
        {
            void DoSomething3();
        }

        #endregion

        #region Test classes

        public class Class1 : IInterface1
        {
            public Class1() { }
            public void DoSomething1() {}
        }

        public class Class2 : IInterface2 {
            public Class2(IInterface1 interface1) {
                Assert.IsNotNull(interface1);
            }

            public void DoSomething2() {}
        }

        public class Class3 : IInterface3
        {
            public Class3(IInterface1 i1, IInterface2 i2) {
                Assert.IsNotNull(i1);
                Assert.IsNotNull(i2);
            }

            public void DoSomething3() { }
        }

        public class Class1_DependsOnIInterface3 : IInterface1
        {
            public Class1_DependsOnIInterface3(IInterface1 i1) {
                Assert.IsNotNull(i1);
            }

            public void DoSomething1() { }
        }

        public class Class1_StringParameter : Class1
        {
            public Class1_StringParameter(string connectionString) {
                Assert.IsNotNull(connectionString);
            }
        }

        public class Class1_MultipleConstructors : Class1 {
            public Class1_MultipleConstructors(IInterface2 i2) {
                Assert.IsNotNull(i2);
            }

            public Class1_MultipleConstructors() {}
        }

        public class Class1_Generic<T> : Class1
        {
            public Class1_Generic(T i) {
                Assert.IsNotNull(i);
            }
        }

        public class Class1_ServiceProviderConstructor : Class1 {
            public Class1_ServiceProviderConstructor(IServiceProvider serviceProvider) {
                Assert.IsNotNull(serviceProvider);
            }
        }

        #endregion
    }
}
