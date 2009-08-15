using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
			container.Kernel.AddComponentInstance<ITestInterface1>(testClass1);
			ITestInterface1 iti = container.Resolve<ITestInterface1>();
			Assert.AreSame(iti, testClass1);
		}

		[Test]
		public void Test2()
		{
			IWindsorContainer container = new WindsorContainer();
			var testClass1 = new TestClass1();
			container.Kernel.AddComponentInstance(typeof(ITestInterface1).FullName, typeof(ITestInterface1), testClass1);
			ITestInterface1 iti = container.Resolve<ITestInterface1>();
			Assert.AreSame(iti, testClass1);
		}


		public interface ITestInterface1 {}
		public class TestClass1 : ITestInterface1 {}
	}
}
