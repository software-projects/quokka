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

using System;
using System.ComponentModel.Design;

namespace Quokka
{
    using NUnit.Framework;

    /// <summary>
    /// For testing out how the ServiceProvider class works.
    /// </summary>
    [TestFixture]
    public class ServiceProviderTests
    {
        #region Interfaces used for testing
        public interface IInterface1
        {
            void DoSomething();
        }

        public interface IInterface2
        {
            void DoSomethingElse();
        }

        public interface IInterface3
        {
            void DoOneMoreThing();
        }

        #endregion

        #region Classes that implement the testing interfaces

        public class Class1 : IInterface1
        {
            public void DoSomething() { }
        }


        public class Class2 : IInterface2
        {
            public void DoSomethingElse() { }
        }

        public class Class3 : IInterface3
        {
            public void DoOneMoreThing() { }
        }

        public class Class3a : IInterface3
        {
            public Class3a(int i) {
                // the purpose of this class is to throw an exception
                // because it does not have a suitable constructor
            }

            public void DoOneMoreThing() { }
        }

        public class Class3b : IInterface3
        {
            private IServiceProvider provider;

            public Class3b(IServiceProvider provider) {
                Assert.IsNotNull(provider);
                Assert.IsNotNull(provider.GetService(typeof(IInterface1)));
                this.provider = provider;
            }

            public void DoOneMoreThing() { }
        }

        #endregion

        [Test]
        public void BasicTest() {

            Class1 class1 = new Class1();
            Class2 class2 = new Class2();

            ServiceContainer container = new ServiceContainer();
            container.AddService(typeof(IInterface1), class1);
            container.AddService(typeof(IInterface2), class2);

            Assert.AreSame(class1, container.GetService(typeof(IInterface1)));
            Assert.AreSame(class2, container.GetService(typeof(IInterface2)));
            Assert.IsNull(container.GetService(typeof(IInterface3)));
        }

        [Test]
        public void NestedContainers() {
            Class1 class1 = new Class1();
            Class2 class2 = new Class2();
            Class3 class3 = new Class3();
            Class1 class1a = new Class1();

            ServiceContainer parentContainer = new ServiceContainer();
            parentContainer.AddService(typeof(IInterface1), class1);

            ServiceContainer childContainer = new ServiceContainer(parentContainer);
            childContainer.AddService(typeof(IInterface2), class2);

            // child container returns what it contains and what its parent contains
            Assert.AreSame(class1, childContainer.GetService(typeof(IInterface1)));
            Assert.AreSame(class2, childContainer.GetService(typeof(IInterface2)));
            Assert.IsNull(childContainer.GetService(typeof(IInterface3)));

            // parent container only returns what it contains
            Assert.IsNull(parentContainer.GetService(typeof(IInterface2)));

            // add a service to the parent, and it becomes available to the child
            parentContainer.AddService(typeof(IInterface3), class3);
            Assert.AreSame(class3, childContainer.GetService(typeof(IInterface3)));
            Assert.AreSame(class3, parentContainer.GetService(typeof(IInterface3)));

            // remove a service from the parent, and it is no longer available to the child
            parentContainer.RemoveService(typeof(IInterface3));
            Assert.IsNull(childContainer.GetService(typeof(IInterface3)));
            Assert.IsNull(parentContainer.GetService(typeof(IInterface3)));

            // remove a service from the child container but not the parent container
            childContainer.RemoveService(typeof(IInterface1));
            Assert.AreSame(class1, childContainer.GetService(typeof(IInterface1)));
            Assert.AreSame(class1, parentContainer.GetService(typeof(IInterface1)));

            // add an implementation to the child container and it overrides the parent container
            childContainer.AddService(typeof(IInterface1), class1a);
            Assert.AreSame(class1a, childContainer.GetService(typeof(IInterface1)));
            Assert.AreSame(class1, parentContainer.GetService(typeof(IInterface1)));

            // remove implementation from the child container, and the implementation in the parent
            // container is used again in the child container
            childContainer.RemoveService(typeof(IInterface1));
            Assert.AreSame(class1, childContainer.GetService(typeof(IInterface1)));
            Assert.AreSame(class1, parentContainer.GetService(typeof(IInterface1)));

            // remove implementation in the child container using promotion 
            childContainer.RemoveService(typeof(IInterface1), true);
            Assert.IsNull(childContainer.GetService(typeof(IInterface1)));
            Assert.IsNull(parentContainer.GetService(typeof(IInterface1)));

            // add implementation to both parent and child container, and then remove from child using
            // promotion, the implementation left in the child will remain
            parentContainer.AddService(typeof(IInterface1), class1);
            childContainer.AddService(typeof(IInterface1), class1a);
            childContainer.RemoveService(typeof(IInterface1), true);
            Assert.AreSame(class1a, childContainer.GetService(typeof(IInterface1)));
            Assert.IsNull(parentContainer.GetService(typeof(IInterface1)));

            // remove using promotion again and the implementation remains in the child
            childContainer.RemoveService(typeof(IInterface1), true);
            Assert.AreSame(class1a, childContainer.GetService(typeof(IInterface1)));
            Assert.AreSame(class1a, childContainer.GetService(typeof(IInterface1)));
            Assert.IsNull(parentContainer.GetService(typeof(IInterface1)));

            // remove without promotion and the instance is gone in the child
            childContainer.RemoveService(typeof(IInterface1), false);
            Assert.IsNull(childContainer.GetService(typeof(IInterface1)));
            Assert.IsNull(parentContainer.GetService(typeof(IInterface1)));
        }

        [Test]
        public void CustomAddService() {
            QuokkaContainer container = new QuokkaContainer();

            container.AddService(typeof(IInterface1), typeof(Class1));
            container.AddService(typeof(IInterface2), typeof(Class2));

            IInterface1 i1 = (IInterface1)container.GetService(typeof(IInterface1));
            Assert.IsNotNull(i1);

            IInterface2 i2 = (IInterface2)container.GetService(typeof(IInterface2));
            Assert.IsNotNull(i2);

            IInterface3 i3 = (IInterface3)container.GetService(typeof(IInterface3));
            Assert.IsNull(i3);

            IInterface1 i1a = (IInterface1)container.GetService(typeof(IInterface1));
            Assert.AreSame(i1, i1a);

            IInterface2 i2a = (IInterface2)container.GetService(typeof(IInterface2));
            Assert.AreSame(i2, i2a);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void NoSuitableConstructor() {
            QuokkaContainer container = new QuokkaContainer();

            // this should throw an exception, because there is no suitable exception for Class3a
            container.AddService(typeof(IInterface3), typeof(Class3a));
        }

        [Test]
        public void ServiceProviderConstructor() {
            QuokkaContainer container = new QuokkaContainer();
            container.AddService(typeof(IInterface3), typeof(Class3b));
            container.AddService(typeof(IInterface1), typeof(Class1));

            Assert.IsNotNull(container.GetService(typeof(IInterface3)));
        }
    }
}
