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
