#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

namespace Quokka
{
    using System;
    using System.ComponentModel.Design;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceContainerUtilTests
    {
        [Test]
        public void BasicTest() {
            ServiceContainer container = new ServiceContainer();

            ServiceContainerUtil.AddService(container, typeof(IInterface1), typeof(Class1));
            ServiceContainerUtil.AddService(container, typeof(IInterface2), typeof(Class2));

            IInterface2 i2 = (IInterface2)container.GetService(typeof(IInterface2));
            Assert.IsNotNull(i2);

            IInterface1 i1 = (IInterface1)container.GetService(typeof(IInterface1));
            Assert.IsNotNull(i1);
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Constructor for Quokka.ServiceContainerUtilTests+Class1_StringParameter has a non-interface parameter: System.String connectionString")]
        public void StringParameterInConstructor() {
            ServiceContainer container = new ServiceContainer();
            ServiceContainerUtil.AddService(container, typeof(IInterface1), typeof(Class1_StringParameter));
        }

        [Test, ExpectedException(typeof(ArgumentException), ExpectedMessage = "Too many constructors for Quokka.ServiceContainerUtilTests+Class1_MultipleConstructors")]
        public void MultipleConstructors() {
            ServiceContainer container = new ServiceContainer();
            ServiceContainerUtil.AddService(container, typeof(IInterface1), typeof(Class1_MultipleConstructors));
        }

        [Test]
        public void GenericClass() {
            ServiceContainer container = new ServiceContainer();
            ServiceContainerUtil.AddService(container, typeof(IInterface1), typeof(Class1_Generic<IInterface2>));
        }

        [Test, ExpectedException(typeof(QuokkaException), ExpectedMessage = "Circular reference detected for Quokka.ServiceContainerUtilTests+IInterface1")]
        public void CircularReference() {
            ServiceContainer container = new ServiceContainer();
            ServiceContainerUtil.AddService(container, typeof(IInterface1), typeof(Class1_DependsOnIInterface3));
            ServiceContainerUtil.AddService(container, typeof(IInterface2), typeof(Class2));
            ServiceContainerUtil.AddService(container, typeof(IInterface3), typeof(Class3));

            IInterface1 i1 = (IInterface1)container.GetService(typeof(IInterface1));
        }

        [Test, ExpectedException(typeof(QuokkaException), ExpectedMessage = "No available implementation of Quokka.ServiceContainerUtilTests+IInterface1")]
        public void MissingType() {
            ServiceContainer container = new ServiceContainer();
            ServiceContainerUtil.AddService(container, typeof(IInterface2), typeof(Class2));
            ServiceContainerUtil.AddService(container, typeof(IInterface3), typeof(Class3));

            IInterface2 i2 = (IInterface2)container.GetService(typeof(IInterface2));
        }

        [Test]
        public void ServiceProviderInConstructor() {
            ServiceContainer container = new ServiceContainer();
            ServiceContainerUtil.AddService(container, typeof(IInterface1), typeof(Class1_ServiceProviderConstructor));

            IInterface1 i1 = (IInterface1)container.GetService(typeof(IInterface1));

        }

        [Test]
        public void ClassImplementsMultipleInterfaces() {
            ServiceContainer container = new ServiceContainer();
            ServiceContainerUtil.AddService(container, typeof(IInterface1), typeof(Class4));
            ServiceContainerUtil.AddService(container, typeof(IInterface2), typeof(Class4));

            IInterface1 i1 = (IInterface1)container.GetService(typeof(IInterface1));
            IInterface2 i2 = (IInterface2)container.GetService(typeof(IInterface2));

            Assert.IsInstanceOfType(typeof(Class4), i1);
            Assert.IsInstanceOfType(typeof(Class4), i2);
            Assert.AreSame(i1, i2);
        }

		[Test]
		public void AddServices()
		{
			ServiceContainer container = new ServiceContainer();

			Type[] types = new Type[] {
					typeof(IInterface1), typeof(Class1),
					typeof(IInterface2), typeof(Class2),
					typeof(IInterface3), typeof(Class3),
			};

			ServiceContainerUtil.AddServices(container, types);

			Assert.IsInstanceOfType(typeof(Class1), container.GetService(typeof(IInterface1)));
			Assert.IsInstanceOfType(typeof(Class2), container.GetService(typeof(IInterface2)));
			Assert.IsInstanceOfType(typeof(Class3), container.GetService(typeof(IInterface3)));
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void AddServicesFailsIfOddNumberOfItems()
		{
			ServiceContainer container = new ServiceContainer();
			Type[] types = new Type[] {
					typeof(IInterface1), typeof(Class1),
					typeof(IInterface2), typeof(Class2),
					typeof(IInterface3), 
			};
			ServiceContainerUtil.AddServices(container, types);
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
            public void DoSomething1() { }
        }

        public class Class2 : IInterface2
        {
            public Class2(IInterface1 interface1) {
                Assert.IsNotNull(interface1);
            }

            public void DoSomething2() { }
        }

        public class Class3 : IInterface3
        {
            public Class3(IInterface1 i1, IInterface2 i2) {
                Assert.IsNotNull(i1);
                Assert.IsNotNull(i2);
            }

            public void DoSomething3() { }
        }

        public class Class4 : IInterface1, IInterface2
        {

            #region IInterface1 Members

            public void DoSomething1() {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion

            #region IInterface2 Members

            public void DoSomething2() {
                throw new Exception("The method or operation is not implemented.");
            }

            #endregion
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

        public class Class1_MultipleConstructors : Class1
        {
            public Class1_MultipleConstructors(IInterface2 i2) {
                Assert.IsNotNull(i2);
            }

            public Class1_MultipleConstructors() { }
        }

        public class Class1_Generic<T> : Class1
        {
            public Class1_Generic(T i) {
                Assert.IsNotNull(i);
            }
        }

        public class Class1_ServiceProviderConstructor : Class1
        {
            public Class1_ServiceProviderConstructor(IServiceProvider serviceProvider) {
                Assert.IsNotNull(serviceProvider);
            }
        }

        #endregion
    }
}
