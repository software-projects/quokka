namespace Quokka.DynamicCodeGeneration
{
	using System;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;
	using NUnit.Framework;
	using Quokka.Uip;

	[TestFixture]
	public class NavigatorProxyBuilderErrorTests
	{
		public enum NavigateValue
		{
			One,
			Two,
			Three
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
			public void NotNavigate() { }
		}

		public interface INavigatorNextBack
		{
			void Next();
			void Back();
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

		private Navigator _inner;
		private AssemblyName assemblyName;
		private string moduleName;
		private string moduleFileName;
		private AssemblyBuilder assemblyBuilder;
		private ModuleBuilder moduleBuilder;

		[SetUp]
		public void SetUp()
		{
			assemblyName = new AssemblyName();
			assemblyName.Name = "NavigatorProxyBuilder2Tests";
			moduleName = assemblyName.Name;
			moduleFileName = moduleName + ".dll";
			assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, moduleFileName);
			_inner = new Navigator();
		}

		[TearDown]
		public void TearDown() {}

		public interface ISupportedInterface
		{
			void Navigate(NavigateValue navigateValue);
			bool CanNavigate(NavigateValue navigateValue);

			void Next();
			bool CanNavigateThree { get; }
		}

		[Test]
		public void CanCreateType()
		{
			NavigatorProxyBuilder builder =
				new NavigatorProxyBuilder(moduleBuilder, "Class1", typeof(ISupportedInterface), typeof(IUipNavigator));

			PrintErrorMessages(builder);
			Assert.IsTrue(builder.CanCreateType);
		}

		public interface INavigateMethodNotVoid
		{
			bool Navigate(NavigateValue navigateValue);
		}

		[Test]
		public void NavigateMethodNotVoid()
		{
			NavigatorProxyBuilder builder =
				new NavigatorProxyBuilder(moduleBuilder, "Class1", typeof(INavigateMethodNotVoid), typeof(IUipNavigator));

			PrintErrorMessages(builder);
			Assert.IsFalse(builder.CanCreateType);
			Assert.AreEqual(1, builder.ErrorMessages.Count);
			Assert.AreEqual("Method 'Navigate' should have a return type of void", builder.ErrorMessages[0]);
		}

		public interface INavigateMethodMissingParameters
		{
			void Navigate();
		}

		[Test]
		public void NavigateMethodMissingParameters()
		{
			NavigatorProxyBuilder builder =
				new NavigatorProxyBuilder(moduleBuilder, "Class1", typeof(INavigateMethodMissingParameters), typeof(IUipNavigator));

			PrintErrorMessages(builder);
			Assert.IsFalse(builder.CanCreateType);
			Assert.AreEqual(1, builder.ErrorMessages.Count);
			Assert.AreEqual("Method 'Navigate' should have one parameter which is an enumerated type", builder.ErrorMessages[0]);
		}

		public interface INavigateMethodTooManyParameters
		{
			void Navigate(NavigateValue a, NavigateValue b);
		}

		[Test]
		public void NavigateMethodTooManyParameters()
		{
			NavigatorProxyBuilder builder =
				new NavigatorProxyBuilder(moduleBuilder, "Class1", typeof(INavigateMethodTooManyParameters), typeof(IUipNavigator));

			PrintErrorMessages(builder);
			Assert.IsFalse(builder.CanCreateType);
			Assert.AreEqual(1, builder.ErrorMessages.Count);
			Assert.AreEqual("Method 'Navigate' should have one parameter which is an enumerated type", builder.ErrorMessages[0]);
		}

		public interface INavigateMethodNotEnumeratedType
		{
			void Navigate(NavigateValue a, NavigateValue b);
		}

		[Test]
		public void NavigateMethodNotEnumeratedType()
		{
			NavigatorProxyBuilder builder =
				new NavigatorProxyBuilder(moduleBuilder, "Class1", typeof(INavigateMethodNotEnumeratedType), typeof(IUipNavigator));

			PrintErrorMessages(builder);
			Assert.IsFalse(builder.CanCreateType);
			Assert.AreEqual(1, builder.ErrorMessages.Count);
			Assert.AreEqual("Method 'Navigate' should have one parameter which is an enumerated type", builder.ErrorMessages[0]);
		}

		[Test]
		public void NavigatorWithProperties()
		{
			Assert.Throws<InvalidOperationException>(() => ProxyFactory.CreateNavigatorProxy<INavigatorWithProperties>(_inner));
		}

		[Test]
		public void NavigatorWithEvent()
		{
			Assert.Throws<InvalidOperationException>(() => ProxyFactory.CreateNavigatorProxy<INavigatorWithEvent>(_inner));
		}

		[Test]
		public void NavigatorWithWrongReturnType()
		{
			Assert.Throws<InvalidOperationException>(() => ProxyFactory.CreateNavigatorProxy<INavigatorWithWrongReturnType>(_inner));
		}

		[Test]
		public void NavigatorWithWrongMethodSignature()
		{
			Assert.Throws<InvalidOperationException>(() => ProxyFactory.CreateNavigatorProxy<INavigatorWithWrongMethodSignature>(_inner));
		}

		[Test]
		public void NavigatorWithMissingMethod()
		{
			Assert.Throws<InvalidOperationException>(() => ProxyFactory.CreateNavigatorProxy<INavigatorNextBack>(new NavigatorWithMissingNavigateMethod()));
		}


		private static void PrintErrorMessages(NavigatorProxyBuilder builder)
		{
			foreach (string errorMessage in builder.ErrorMessages) {
				Console.WriteLine(errorMessage);
			}
		}
	}
}