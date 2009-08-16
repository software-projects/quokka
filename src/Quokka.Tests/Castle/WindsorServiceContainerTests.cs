using Castle.Windsor;
using Microsoft.Practices.ServiceLocation;
using NUnit.Framework;
using Quokka.ServiceLocation;

namespace Quokka.Castle
{
	[TestFixture]
	public class WindsorServiceContainerTests
	{
		[Test]
		public void Container_supports_IWindsorContainer_interface()
		{
			IServiceLocator locator = ServiceContainerFactory.CreateContainer().Locator;
			IWindsorContainer container1 = locator.GetInstance<IWindsorContainer>();
			Assert.IsNotNull(container1);
			IWindsorContainer container2 = locator.GetInstance<IWindsorContainer>();
			Assert.AreSame(container1, container2);
		}

		[Test]
		public void Container_supports_IServiceContainer_interface()
		{
			IServiceLocator locator = ServiceContainerFactory.CreateContainer().Locator;
			IServiceContainer container1 = locator.GetInstance<IServiceContainer>();
			Assert.IsNotNull(container1);
			IServiceContainer container2 = locator.GetInstance<IServiceContainer>();
			Assert.AreSame(container1, container2);
		}

		[Test]
		public void IsTypeRegistered()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			container.RegisterType<IInterface1, Class1>(ServiceLifecycle.Singleton);
			Assert.IsTrue(container.IsTypeRegistered<IInterface1>());
			Assert.IsFalse(container.IsTypeRegistered<IInterface2>());
			Assert.IsFalse(container.IsTypeRegistered<IInterface1>("XXX"));
			container.RegisterType<IInterface1, Class1>("XXX", ServiceLifecycle.PerRequest);
			Assert.IsTrue(container.IsTypeRegistered<IInterface1>("XXX"));

			container.RegisterType<IInterface1, Class1>("someName", ServiceLifecycle.PerRequest);
			container.RegisterType<IInterface2, Class2>("someOtherName", ServiceLifecycle.Singleton);

			// This fails -- cannot register two different objects with the same name even if they have
			// different interface types. This differes to unity container.
			//container.RegisterType<IInterface2, Class2>("someName", ServiceLifecycle.Singleton);

			Assert.IsTrue(container.IsTypeRegistered<IInterface1>("someName"));

			Assert.IsTrue(container.IsTypeRegistered<IInterface2>());


			// this fails
			//Assert.IsFalse(container.IsTypeRegistered<IInterface1>("someOtherName"));
			
			Assert.IsTrue(container.IsTypeRegistered<IInterface2>("someOtherName"));


		}

		[Test]
		public void NestedServiceContainers()
		{
			IServiceLocator locator = ServiceContainerFactory.CreateContainer().Locator;
			IServiceContainer parentContainer = locator.GetInstance<IServiceContainer>();

			Class2 c2 = new Class2();
			parentContainer.RegisterInstance(typeof (IInterface2), c2);

			IServiceContainer childContainer = parentContainer.CreateChildContainer();
			childContainer.RegisterType<IInterface1, Class1>(ServiceLifecycle.PerRequest);

			Assert.AreSame(c2, childContainer.Locator.GetService(typeof (IInterface2)));

			IInterface1 i1 = (IInterface1) childContainer.Locator.GetService(typeof (IInterface1));
			Assert.IsNotNull(i1);
		}

		public interface IInterface1
		{
		}

		public interface IInterface2
		{
		}

		public class Class1 : IInterface1
		{
		}

		public class Class2 : IInterface2
		{
		}
	}
}