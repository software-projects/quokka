using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Quokka.Sandbox
{
	[TestFixture]
	public class ReflectionSpikes
	{
		[Test]
		public void Get_type_from_text()
		{
			string typeName = "Castle.Core.CollectionExtensions,Castle.Core";
			Type type = Type.GetType(typeName);
			Assert.IsNotNull(type);

			Console.WriteLine(type.AssemblyQualifiedName);

			var assemblyName = type.Assembly.GetName();

			Console.WriteLine(type.FullName + "," + assemblyName.Name);

		}

		[Test]
		public void Create_type_without_assembly_name()
		{
			var expectedType = typeof (global::Castle.Core.CollectionExtensions);
			string typeName = "Castle.Core.CollectionExtensions";
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
