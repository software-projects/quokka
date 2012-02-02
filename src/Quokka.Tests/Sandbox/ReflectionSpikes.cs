using System;
using NUnit.Framework;

namespace Quokka.Sandbox
{
	[TestFixture]
	public class ReflectionSpikes
	{
		private Type _type;
		[SetUp]
		public void SetUp()
		{
			// ensure castle.core assembly is loaded in the app domain
			_type = typeof (global::Castle.Core.Logging.ILogger);
		}

		[Test]
		public void Get_type_from_text()
		{
			string typeName = "Castle.Core.IServiceProviderEx,Castle.Core";
			Type type = Type.GetType(typeName);
			Assert.IsNotNull(type);

			Console.WriteLine(type.AssemblyQualifiedName);

			var assemblyName = type.Assembly.GetName();

			Console.WriteLine(type.FullName + "," + assemblyName.Name);

		}

		[Test]
		public void Create_type_without_assembly_name()
		{
			var expectedType = typeof (global::Castle.Core.IServiceProviderEx);
			string typeName = "Castle.Core.IServiceProviderEx";
			Type type = null;

			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				var assemblyName = assembly.GetName();
				if (typeName.StartsWith(assemblyName.Name))
				{
					var assemblyQualifiedName = typeName + "," + assembly.GetName().FullName;
					type = Type.GetType(assemblyQualifiedName, false);
					if (type != null)
					{
						break;
					}
				}
			}

			Assert.AreSame(expectedType, type);
		}
	}
}
