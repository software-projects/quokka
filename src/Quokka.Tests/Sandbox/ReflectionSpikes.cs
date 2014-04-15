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
