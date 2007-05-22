namespace Quokka.DynamicCodeGeneration
{
	using System;
	using NUnit.Framework;
	using Quokka.Uip;

	[TestFixture]
	public class NavigatorProxyTests
	{
		private Navigator _inner;

		[SetUp]
		public void SetUp()
		{
			_inner = new Navigator();
		}

		public class Navigator : IUipNavigator
		{
			public string LastMethodCalled;

			public bool CanNavigate(string navigateValue)
			{
				throw new NotImplementedException();
			}

			public void Navigate(string navigateValue)
			{
				LastMethodCalled = navigateValue;
			}
		}

		public interface INavigator
		{
			void Next();
			void Back();
		}

		public interface INavigatorWithProperties
		{
			void Next();
			string Property { get; }
		}

		public interface INavigatorWithEvent
		{
			void Next();
			event EventHandler SomeEvent;
		}

		public interface INavigatorWithWrongReturnType
		{
			string Next();
		}

		public interface INavigatorWithWrongMethodSignature
		{
			void Next(string s);
		}

		public class NavigatorWithMissingNavigateMethod
		{
			public void NotNavigate() {}
		}

		[Test]
		public void Navigate()
		{
			INavigator navigator = ProxyFactory.CreateNavigatorProxy<INavigator>(_inner);

			Assert.IsNull(_inner.LastMethodCalled);
			navigator.Next();
			Assert.AreEqual("Next", _inner.LastMethodCalled);
			navigator.Back();
			Assert.AreEqual("Back", _inner.LastMethodCalled);
			navigator.Next();
			Assert.AreEqual("Next", _inner.LastMethodCalled);
		}

		[Test]
		public void WrapperTypeCache()
		{
			Type wrapperType1 = ProxyFactory.GetNavigatorProxyType(typeof(INavigator), typeof(Navigator));
			Type wrapperType2 = ProxyFactory.GetNavigatorProxyType(typeof(INavigator), typeof(Navigator));
			Assert.AreSame(wrapperType1, wrapperType2);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NavigatorWithProperties()
		{
			ProxyFactory.CreateNavigatorProxy<INavigatorWithProperties>(_inner);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NavigatorWithEvent()
		{
			ProxyFactory.CreateNavigatorProxy<INavigatorWithEvent>(_inner);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NavigatorWithWrongReturnType()
		{
			ProxyFactory.CreateNavigatorProxy<INavigatorWithWrongReturnType>(_inner);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NavigatorWithWrongMethodSignature()
		{
			ProxyFactory.CreateNavigatorProxy<INavigatorWithWrongMethodSignature>(_inner);
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void NavigatorWithMissingMethod()
		{
			ProxyFactory.CreateNavigatorProxy<INavigator>(new NavigatorWithMissingNavigateMethod());
		}
	}
}