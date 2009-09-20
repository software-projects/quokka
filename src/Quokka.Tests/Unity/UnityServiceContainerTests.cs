using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using NUnit.Framework;
using Quokka.ServiceLocation;

namespace Quokka.Unity
{
	[TestFixture]
	public class UnityServiceContainerTests
	{
		[Test]
		public void Container_supports_IUnityContainer_interface()
		{
			IServiceLocator locator = ServiceContainerFactory.CreateContainer().Locator;
			IUnityContainer container1 = locator.GetInstance<IUnityContainer>();
			Assert.IsNotNull(container1);
			IUnityContainer container2 = locator.GetInstance<IUnityContainer>();
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
		public void Container_behaviour_tests()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			container.RegisterType(typeof(IInterface1), typeof(Class1), "xxx", ServiceLifecycle.PerRequest);
			container.RegisterType(typeof(IInterface2), typeof(Class2), "xxx", ServiceLifecycle.PerRequest);
		}

		[Test]
		public void IsTypeRegistered()
		{
			IServiceContainer container = ServiceContainerFactory.CreateContainer();
			container.RegisterType<IInterface1, Class1>(ServiceLifecycle.Singleton);
			Assert.IsTrue(container.IsTypeRegistered<IInterface1>());
			Assert.IsFalse(container.IsTypeRegistered<IInterface2>());
		}

		/// <summary>
		/// This test is really a bit of a 'spike' to verify that Unity works
		/// the way we expect.
		/// </summary>
		[Test]
		public void NestedUnityContainers()
		{
			IUnityContainer parentContainer = new UnityContainer();


			Class2 c2 = new Class2();
			parentContainer.RegisterInstance(typeof (IInterface2), c2);
			parentContainer.RegisterType<IInterface1, Class1>();

			IUnityContainer childContainer = parentContainer.CreateChildContainer();

			Assert.AreSame(c2, childContainer.Resolve<IInterface2>());

			IInterface1 i1 = childContainer.Resolve<IInterface1>();
			Assert.IsNotNull(i1);
		}

		[Test]
		public void NestedServiceContainers()
		{
			IServiceLocator locator = ServiceContainerFactory.CreateContainer().Locator;
			IServiceContainer parentContainer = locator.GetInstance<IServiceContainer>();

			Class2 c2 = new Class2();
			parentContainer.RegisterInstance(typeof(IInterface2), c2);

			IServiceContainer childContainer = parentContainer.CreateChildContainer();
			childContainer.RegisterType<IInterface1, Class1>(ServiceLifecycle.PerRequest);

			Assert.AreSame(c2, childContainer.Locator.GetService(typeof(IInterface2)));

			IInterface1 i1 = (IInterface1)childContainer.Locator.GetService(typeof (IInterface1));
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