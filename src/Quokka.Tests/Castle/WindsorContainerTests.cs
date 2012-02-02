using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;

namespace Quokka.Castle
{
	[TestFixture]
	public class WindsorContainerTests
	{
		[Test]
		public void Test1()
		{
			IWindsorContainer container = new WindsorContainer();
			var testClass1 = new TestClass1();
			container.Kernel.Register(Component.For<ITestInterface1>().Instance(testClass1));
			ITestInterface1 iti = container.Resolve<ITestInterface1>();
			Assert.AreSame(iti, testClass1);
		}

		[Test]
		public void Test2()
		{
			IWindsorContainer container = new WindsorContainer();
			var testClass1 = new TestClass1();
			container.Kernel.Register(
				Component.For(typeof (ITestInterface1)).Named(typeof (ITestInterface1).FullName).Instance(testClass1));
			ITestInterface1 iti = container.Resolve<ITestInterface1>();
			Assert.AreSame(iti, testClass1);
		}


		public interface ITestInterface1 {}
		public class TestClass1 : ITestInterface1 {}
	}
}
