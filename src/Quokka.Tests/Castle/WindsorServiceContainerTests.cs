using Castle.Windsor;
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
		public void Container_supports_per_request()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			container.RegisterType<IInterface1, Class1>(ServiceLifecycle.PerRequest);
			IInterface1 i1 = container.Locator.GetInstance<IInterface1>();
			IInterface1 i2 = container.Locator.GetInstance<IInterface1>();
			Assert.AreNotSame(i1, i2);
		}

		[Test]
		public void Container_supports_singleton()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			container.RegisterType<IInterface1, Class1>(ServiceLifecycle.Singleton);
			IInterface1 i1 = container.Locator.GetInstance<IInterface1>();
			IInterface1 i2 = container.Locator.GetInstance<IInterface1>();
			Assert.AreSame(i1, i2);
		}

		[Test]
		public void Can_register_concrete_singleton_instance()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			container.RegisterType<Class1>(ServiceLifecycle.Singleton);
			var c1 = container.Locator.GetInstance<Class1>();
			var c2 = container.Locator.GetInstance<Class1>();
			Assert.IsNotNull(c1);
			Assert.IsNotNull(c2);
			Assert.AreSame(c1, c2);
		}

		[Test]
		public void Can_create_transient_class_instance_without_registration()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			var c1 = container.Locator.GetInstance<Class1>();
			var c2 = container.Locator.GetInstance<Class1>();
			Assert.IsNotNull(c1);
			Assert.IsNotNull(c2);
			Assert.AreNotSame(c1, c2);
		}

		[Test]
		public void IsTypeRegistered()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			container.RegisterType<IInterface1, Class1>(ServiceLifecycle.Singleton);
			Assert.IsTrue(container.IsTypeRegistered<IInterface1>());
			Assert.IsFalse(container.IsTypeRegistered<IInterface2>());

			IServiceContainer container2 = ServiceContainerFactory.CreateContainer();
			container2.RegisterType<IInterface1, Class1>("XXX", ServiceLifecycle.PerRequest);
			Assert.IsTrue(container.IsTypeRegistered<IInterface1>());

			container.RegisterType<IInterface1, Class1>("someName", ServiceLifecycle.PerRequest);
			container.RegisterType<IInterface2, Class2>("someOtherName", ServiceLifecycle.Singleton);
			Assert.IsTrue(container.IsTypeRegistered<IInterface2>());

		
			Assert.IsTrue(container.IsTypeRegistered<IInterface2>());
			Assert.IsNotNull(container.Locator.GetInstance<IInterface2>());
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