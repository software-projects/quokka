using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using NUnit.Framework;
using Quokka.NH.Interfaces;
using Quokka.NH.Startup;

// ReSharper disable InconsistentNaming
namespace Quokka.NH.Tests
{
	[TestFixture]
	public class DefaultAliasContributorTests
	{
		[Test]
		public void Injects_default_alias()
		{
			using (var container = new WindsorContainer())
			{
				container.AddFacility<NHibernateFacility>(f => f.DefaultAlias = "XYZ");
				container.Register(
					Component.For<WantDefaultAlias>()
						.LifestyleTransient());

				var component = container.Resolve<WantDefaultAlias>();
				Assert.AreEqual("XYZ", component.DefaultAlias);
			}
		}

		[Test]
		public void Leaves_component_alone_if_interface_not_implemented()
		{
			using (var container = new WindsorContainer())
			{
				container.AddFacility<NHibernateFacility>(f => f.DefaultAlias = "XYZ");
				container.Register(
					Component.For<DontWantDefaultAlias>()
						.LifestyleTransient());

				var component = container.Resolve<DontWantDefaultAlias>();
				Assert.IsNull(component.DefaultAlias);
			}
		}

		[Test]
		public void Injects_new_default_alias_if_it_changes()
		{
			using (var container = new WindsorContainer())
			{
				var facility = new NHibernateFacility {DefaultAlias = "123"};
				container.AddFacility(facility);
				container.Register(
					Component.For<WantDefaultAlias>()
						.LifestyleTransient());

				var component = container.Resolve<WantDefaultAlias>();
				Assert.AreEqual("123", component.DefaultAlias);

				facility.DefaultAlias = "456";

				var component2 = container.Resolve<WantDefaultAlias>();
				Assert.AreEqual("456", component2.DefaultAlias);
			}
		}

		[Test]
		public void Default_alias_can_be_null()
		{
			using (var container = new WindsorContainer())
			{
				container.AddFacility<NHibernateFacility>(f => f.DefaultAlias = null);
				container.Register(
					Component.For<WantDefaultAlias>()
						.LifestyleTransient());

				var component = container.Resolve<WantDefaultAlias>();
				Assert.IsNull(component.DefaultAlias);
			}
		}

		public class WantDefaultAlias : IDefaultAlias
		{
			public string DefaultAlias { get; set; }
		}

		public class DontWantDefaultAlias
		{
			public string DefaultAlias { get; set; }
		}
	}
}
