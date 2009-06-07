using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Quokka.Unity;

// suppress warnings about obsolete usage
#pragma warning disable 612,618

namespace Quokka.Reflection
{
	using System.ComponentModel.Design;

	[TestFixture]
    public class ObjectFactoryTests
    {
		private IServiceLocator serviceLocator;

		[SetUp]
		public void SetUp()
		{
			serviceLocator = ServiceContainerFactory.CreateContainer().Locator;
			ServiceLocator.SetLocatorProvider(() => serviceLocator );
		}

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Create_ArgumentNull_1() {
            object obj = ObjectFactory.Create(null, null, null);
        }

        [Test]
        public void NoConstructorsDefined() {
            object obj = ObjectFactory.Create(typeof(MockNoConstructorsDefined), null);
            Assert.IsInstanceOfType(typeof(MockNoConstructorsDefined), obj);
        }

        [Test]
        [ExpectedException(typeof(QuokkaException))]
        public void NoPublicConstructor() {
            object obj = ObjectFactory.Create(typeof(MockNoPublicConstructor), null);
        }

        [Test]
        public void ConstructorString() {
            object obj = ObjectFactory.Create(typeof(MockConstructorString), null, "XYZ");
            Assert.IsInstanceOfType(typeof(MockConstructorString), obj);
            MockConstructorString mock = (MockConstructorString)obj;
            Assert.AreEqual("XYZ", mock.StringValue);
        }

        [Test]
        public void UseServiceProvider() {
            MockClass1 mock1 = new MockClass1();
            MockClass2 mock2 = new MockClass2();
            MockClass3 mock3 = new MockClass3();
            ServiceContainer container = new ServiceContainer();
            container.AddService(typeof(IMockInterface1), mock1);
            container.AddService(typeof(IMockInterface2), mock2);
            object obj = ObjectFactory.Create(typeof(MockConstructorInterfaces), container, mock3);
            Assert.IsInstanceOfType(typeof(MockConstructorInterfaces), obj);
            MockConstructorInterfaces mock = (MockConstructorInterfaces)obj;

            Assert.AreSame(mock1, mock.Mock1);
            Assert.AreSame(mock2, mock.Mock2);
            Assert.AreSame(mock3, mock.Mock3);
        }

        [Test]
        public void NullInConcreteObjects() {
            MockClass1 mock1 = new MockClass1();
            MockClass2 mock2 = new MockClass2();
            MockClass3 mock3 = new MockClass3();
            ServiceContainer container = new ServiceContainer();
            container.AddService(typeof(IMockInterface1), mock1);
            container.AddService(typeof(IMockInterface2), mock2);
            object obj = ObjectFactory.Create(typeof(MockConstructorInterfaces), container, mock3, null);
            Assert.IsInstanceOfType(typeof(MockConstructorInterfaces), obj);
            MockConstructorInterfaces mock = (MockConstructorInterfaces)obj;

            Assert.AreSame(mock1, mock.Mock1);
            Assert.AreSame(mock2, mock.Mock2);
            Assert.AreSame(mock3, mock.Mock3);
        }

        [Test]
        public void RequiresServiceProvider() {
            ServiceContainer container = new ServiceContainer();
            MockRequiresServiceProvider mock = (MockRequiresServiceProvider)ObjectFactory.Create(typeof(MockRequiresServiceProvider), container);
            Assert.AreSame(container, mock.ServiceProvider);
        }

        #region Mock classes and interfaces used by this test fixture

        public class MockNoConstructorsDefined
        {
        }

        public class MockNoPublicConstructor
        {
            internal MockNoPublicConstructor() { }
        }

        public class MockConstructorString
        {
            public readonly string StringValue;

            public MockConstructorString(string s) {
                StringValue = s;
            }
        }

        public class MockConstructorInterfaces
        {
            public readonly IMockInterface1 Mock1;
            public readonly IMockInterface2 Mock2;
            public readonly MockClass3 Mock3;

            public MockConstructorInterfaces(IMockInterface1 mock1, IMockInterface2 mock2, MockClass3 mock3) {
                Mock1 = mock1;
                Mock2 = mock2;
                Mock3 = mock3;
            }
        }

        public interface IMockInterface1 { }
        public interface IMockInterface2 { }

        public class MockClass1 : IMockInterface1 { }
        public class MockClass2 : IMockInterface2 { }
        public class MockClass3 {}

        public class MockRequiresServiceProvider
        {
            public readonly IServiceProvider ServiceProvider;

            public MockRequiresServiceProvider(IServiceProvider serviceProvider) {
                Assert.IsNotNull(serviceProvider);
                ServiceProvider = serviceProvider;
            }
        }

        #endregion
    }
}
// restore warnings about obsolete usage
#pragma warning restore 612,618
