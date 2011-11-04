namespace Quokka.DynamicCodeGeneration
{
	using System;
	using System.Collections.Generic;
	using NUnit.Framework;
	using Quokka.Uip;

	[TestFixture]
	[Ignore("NavigatorProxy fails tests on .NET 4.0, but is no longer used")]
	public class NavigatorProxyTests : CodeBuilderTestBase
	{
		private Navigator _inner;

		[SetUp]
		public void SetUp()
		{
			_inner = new Navigator();
			CreateAssemblyBuilder();
		}

		[TearDown]
		public void TearDown()
		{
			VerifyAssembly();
		}

		public class Navigator : IUipNavigator
		{
			public string LastMethodCalled;
			private readonly List<string> navigateValues = new List<string>();

			public bool CanNavigate(string navigateValue)
			{
				return navigateValues.Contains(navigateValue);
			}

			public void Navigate(string navigateValue)
			{
				LastMethodCalled = navigateValue;
			}

			public void AddNavigateValues(params string[] values)
			{
				navigateValues.Clear();
				navigateValues.AddRange(values);
			}
		}

		public interface INavigatorNextBack
		{
			void Next();
			void Back();
			bool CanNavigateNext { get; }
		}

		[Test]
		public void NavigateNextBack()
		{
			NavigatorProxyBuilder builder =
				new NavigatorProxyBuilder(moduleBuilder, "Class1", typeof(INavigatorNextBack), typeof(Navigator));
			Assert.IsTrue(builder.CanCreateType);

			Type proxyType = builder.CreateType();
			INavigatorNextBack navigator = CreateProxy<INavigatorNextBack, Navigator>(proxyType, _inner);

			Assert.IsNull(_inner.LastMethodCalled);
			navigator.Next();
			Assert.AreEqual("Next", _inner.LastMethodCalled);
			navigator.Back();
			Assert.AreEqual("Back", _inner.LastMethodCalled);
			navigator.Next();
			Assert.AreEqual("Next", _inner.LastMethodCalled);

			Assert.IsFalse(navigator.CanNavigateNext);
			_inner.AddNavigateValues("Next");
			Assert.IsTrue(navigator.CanNavigateNext);

		}

		public enum NavigateValue
		{
			One,
			Two,
			Three
		}

		public interface INavigatorEnum
		{
			bool CanNavigate(NavigateValue navigateValue);
			void Navigate(NavigateValue navigateValue);
		}

		[Test]
		public void NavigatorEnum()
		{
			NavigatorProxyBuilder builder =
				new NavigatorProxyBuilder(moduleBuilder, "Class1", typeof(INavigatorEnum), typeof(Navigator));
			Assert.IsTrue(builder.CanCreateType);

			Type proxyType = builder.CreateType();
			INavigatorEnum navigator = CreateProxy<INavigatorEnum, Navigator>(proxyType, _inner);

			_inner.AddNavigateValues("One", "Two");
			Assert.IsTrue(navigator.CanNavigate(NavigateValue.One));
			Assert.IsTrue(navigator.CanNavigate(NavigateValue.Two));
			Assert.IsFalse(navigator.CanNavigate(NavigateValue.Three));

			Assert.IsNull(_inner.LastMethodCalled);

			navigator.Navigate(NavigateValue.One);
			Assert.AreEqual("One", _inner.LastMethodCalled);

			navigator.Navigate(NavigateValue.Two);
			Assert.AreEqual("Two", _inner.LastMethodCalled);
		}

		public interface INavigatorMix
		{
			bool CanNavigate(NavigateValue navigateValue);
			void Navigate(NavigateValue navigateValue);

			bool CanNavigateTwo { get; }
			void One();
		}

		[Test]
		public void NavigatorMix()
		{
			NavigatorProxyBuilder builder =
				new NavigatorProxyBuilder(moduleBuilder, "Class1", typeof(INavigatorMix), typeof(Navigator));
			Assert.IsTrue(builder.CanCreateType);

			Type proxyType = builder.CreateType();
			INavigatorMix navigator = CreateProxy<INavigatorMix, Navigator>(proxyType, _inner);

			Assert.IsFalse(navigator.CanNavigateTwo);
			Assert.IsFalse(navigator.CanNavigate(NavigateValue.One));
			Assert.IsFalse(navigator.CanNavigate(NavigateValue.Two));
			Assert.IsFalse(navigator.CanNavigate(NavigateValue.Three));

			_inner.AddNavigateValues("One", "Two");
			Assert.IsTrue(navigator.CanNavigate(NavigateValue.One));
			Assert.IsTrue(navigator.CanNavigate(NavigateValue.Two));
			Assert.IsFalse(navigator.CanNavigate(NavigateValue.Three));
			Assert.IsTrue(navigator.CanNavigateTwo);

			Assert.IsNull(_inner.LastMethodCalled);

			navigator.Navigate(NavigateValue.One);
			Assert.AreEqual("One", _inner.LastMethodCalled);

			navigator.Navigate(NavigateValue.Two);
			Assert.AreEqual("Two", _inner.LastMethodCalled);

			navigator.One();
			Assert.AreEqual("One", _inner.LastMethodCalled);
		}
	}
}