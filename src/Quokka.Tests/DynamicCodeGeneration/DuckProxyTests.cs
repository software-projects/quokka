using System;
using NUnit.Framework;

namespace Quokka.DynamicCodeGeneration
{
	[TestFixture]
	public class DuckProxyTests
	{
		public interface ITest1 {
			void SimpleMethod();

			double MoreComplexMethod(Inner1 a, int b, int c, float d, double e);

			int ReadOnlyProperty { get; }
			string ReadWriteProperty { get; set; }
			void NotImplementedMethod();
		}

		public class Inner1 {
			public bool simpleMethodCalled = false;
			public int readOnlyProperty = 0;
			public string readWriteProperty = null;

			public void SimpleMethod() {
				simpleMethodCalled = true;
			}

			public int ReadOnlyProperty {
				get { return this.readOnlyProperty; }
			}

			public string ReadWriteProperty {
				get { return this.readWriteProperty; }
				set { this.readWriteProperty = value; }
			}

			public double MoreComplexMethod(Inner1 a, int b, int c, float d, double e) {
				Assert.AreSame(this, a);

				double sum = b + c + d + e;
				return sum;
			}

		}


		[Test]
		public void SimpleMethod() {
			Inner1 inner = new Inner1();
			ITest1 i = ProxyFactory.CreateDuckProxy<ITest1>(inner);
			i.SimpleMethod();
			Assert.IsTrue(inner.simpleMethodCalled);
		}

		[Test]
		public void ReadOnlyProperty() {
			Inner1 inner = new Inner1();
			inner.readOnlyProperty = 42;

            ITest1 i = ProxyFactory.CreateDuckProxy<ITest1>(inner);
			Assert.AreEqual(inner.ReadOnlyProperty, i.ReadOnlyProperty);
		}

		[Test]
		public void ReadWriteProperty() {
			Inner1 inner = new Inner1();
			inner.readWriteProperty = "Value 1";

            ITest1 i = ProxyFactory.CreateDuckProxy<ITest1>(inner);
			Assert.AreEqual(inner.ReadWriteProperty, i.ReadWriteProperty);
			i.ReadWriteProperty = "Value 2";
			Assert.AreEqual("Value 2", inner.ReadWriteProperty);
		}

		[Test]
		public void NotImplementedException() {
			Inner1 inner = new Inner1();
            ITest1 i = ProxyFactory.CreateDuckProxy<ITest1>(inner);
			Assert.Throws<NotSupportedException>(() => i.NotImplementedMethod());
		}

		[Test]
		public void MoreComplexMethod() {
			Inner1 inner = new Inner1();
            ITest1 i = ProxyFactory.CreateDuckProxy<ITest1>(inner);

			int b = 5;
			int c = 6;
			float d = 27.5f;
			double e = 2.7182818;

			double sum = i.MoreComplexMethod(inner, b, c, d, e);
			Assert.AreEqual(b + c + d + e, sum);
		}

		[Test]
		public void WrapperTypeCache() {
			Type wrapperType1 = ProxyFactory.GetDuckProxyType(typeof(ITest1), typeof(Inner1));
			Type wrapperType2 = ProxyFactory.GetDuckProxyType(typeof(ITest1), typeof(Inner1));
			Assert.AreSame(wrapperType1, wrapperType2);
		}

		public interface ITest2 {
			void DoNothing();
		}

		public class Inner2 : ITest2 {
			public void DoNothing() {}
		}

		[Test]
		public void NoWrapperNeeded() {
			Inner2 c2 = new Inner2();
            ITest2 i2 = ProxyFactory.CreateDuckProxy<ITest2>(c2);

			// should not create a wrapper when the class already implements
			// the interface
			Assert.AreSame(c2, i2);

			Inner1 c1 = new Inner1();
            ITest1 i1a = ProxyFactory.CreateDuckProxy<ITest1>(c1);
            ITest1 i1b = ProxyFactory.CreateDuckProxy<ITest1>(i1a);

			// a wrapper of a wrapper is the same object
			Assert.AreSame(i1a, i1b);
		}

		public interface ITest3 {
			object Method1();
			void Method2();
			void Method3();
			void Method4();
		}

		public class Inner3 {
			public bool Method1Called = false;
			public bool Method2Called = false;
			public bool Method3Called = false;
			public bool Method4Called = false;

			public object Method1() {
				Method1Called = true;
				return "Some result";
			}

			public void Method2() {
				Method2Called = true;
			}

			public void Method3() {
				Method3Called = true;
			}

			public void Method4() {
				Method4Called = true;
			}
		}

		[Test]
		public void InterfaceWithNoProperties() {
			Inner3 inner = new Inner3();
			ITest3 i = ProxyFactory.CreateDuckProxy<ITest3>(inner);
			
			Assert.IsNotNull(i.Method1());
			i.Method2();
			i.Method3();
			i.Method4();
				
			Assert.IsTrue(inner.Method1Called);
			Assert.IsTrue(inner.Method2Called);
			Assert.IsTrue(inner.Method3Called);
			Assert.IsTrue(inner.Method4Called);
		}

        public interface IGenericInterface<T>
        {
            T DoSomething(T t);
        }

        public class Inner4
        {
            public int DoSomething(int i) {
                return i * 3;
            }
        }

        [Test]
        public void GenericInterface() {
            Inner4 inner = new Inner4();

            IGenericInterface<int> i = ProxyFactory.CreateDuckProxy<IGenericInterface<int>>(inner);

            Assert.AreEqual(9, i.DoSomething(3));
        }

        public interface ITest5
        {
            event EventHandler SomethingHappened;
        }

        public class Inner5 {
            public event EventHandler SomethingHappened;

            public void RaiseSomethingHappened() {
                if (SomethingHappened != null) {
                    SomethingHappened(this, EventArgs.Empty);
                }
            }
        }

        [Test]
        public void SimpleEvent() {
            Inner5 inner = new Inner5();

            ITest5 i = ProxyFactory.CreateDuckProxy<ITest5>(inner);

            bool eventRaised = false;

            i.SomethingHappened += delegate(object sender, EventArgs e) {
                eventRaised = true;
            };

            inner.RaiseSomethingHappened();

            Assert.IsTrue(eventRaised);
        }

        public class Test6EventArgs : EventArgs {
            public int IntValue;
            public string StringValue;
        }

        public interface ITest6
        {
            event EventHandler<Test6EventArgs> SomethingHappened;
        }

        public class Inner6
        {
            public event EventHandler<Test6EventArgs> SomethingHappened;

            public void RaiseSomethingHappened(int intValue, string stringValue) {
                if (SomethingHappened != null) {
                    Test6EventArgs e = new Test6EventArgs();
                    e.IntValue = intValue;
                    e.StringValue = stringValue;
                }
            }
        }

        [Test]
        public void GenericEvent() {
            Inner6 inner = new Inner6();
            ITest6 i = ProxyFactory.CreateDuckProxy<ITest6>(inner);

            bool eventRaised = true;

            i.SomethingHappened += delegate(object sender, Test6EventArgs e) {
                Assert.AreEqual(42, e.IntValue);
                Assert.AreEqual("Forty Two", e.StringValue);
                eventRaised = true;
            };

            inner.RaiseSomethingHappened(42, "FortyTwo");
            Assert.IsTrue(eventRaised);
        }

        [Test]
        public void NotAnInterface() {
            Inner1 inner = new Inner1();
            Assert.Throws<ArgumentException>(() => ProxyFactory.CreateDuckProxy<Inner2>(inner));
            
        }

        [Test]
        public void NullInner() {
            ITest1 i = ProxyFactory.CreateDuckProxy<ITest1>(null);
            Assert.IsNull(i);
        }

        public interface ITest7
        {
            int ReturnSomething();
            void DoSomething();
        }

        public class Inner7
        {
            public bool DoneSomething;
            public bool ReturnSomething() { return true; }
            public void DoSomething() { DoneSomething = true; }
        }

        // Test for matching method with incompatible return types.
        // The other methods defined should still work.
        [Test]
        public void MismatchedReturnType_1() {
            Inner7 inner = new Inner7();
            ITest7 i7 = ProxyFactory.CreateDuckProxy<ITest7>(inner);

            Assert.IsFalse(inner.DoneSomething);
            i7.DoSomething();
            Assert.IsTrue(inner.DoneSomething);
        }

        // Test for matching method with incompatible return types.
        // Invoking the method throws NotSupportedException.
        [Test]
        public void MismatchedReturnType_2() {
            Inner7 inner = new Inner7();

			ITest7 i7 = ProxyFactory.CreateDuckProxy<ITest7>(inner);

			Assert.Throws<NotSupportedException>(() => i7.ReturnSomething());
        }

        public interface Interface8
        {
            bool CanDoSomething { get; }
            bool DoSomething();
            bool IsDoSomethingSupported { get; }
        }

        public class Inner8a
        {
            public bool DoSomething() {
                return true; 
            }
        }

        public class Inner8b
        {
            public void DoesNotDoAnything() {}
        }

        [Test]
        public void CanCheckWhetherAMethodIsSupported()
        {
            Inner8a isSupported = new Inner8a();
            Interface8 i8 = ProxyFactory.CreateDuckProxy<Interface8>(isSupported);

            Assert.IsTrue(i8.CanDoSomething);
            Assert.IsTrue(i8.DoSomething());
            Assert.IsTrue(i8.IsDoSomethingSupported);

            Inner8b notSupported = new Inner8b();
            i8 = ProxyFactory.CreateDuckProxy<Interface8>(notSupported);

            Assert.IsFalse(i8.CanDoSomething);
            Assert.IsFalse(i8.IsDoSomethingSupported);

            try
            {
                i8.DoSomething();
                Assert.Fail();
            }
            catch (NotSupportedException)
            {
            }
        }
	}
}
