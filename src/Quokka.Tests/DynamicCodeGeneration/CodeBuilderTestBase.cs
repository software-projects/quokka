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

namespace Quokka.DynamicCodeGeneration
{
	using System;
	using System.Diagnostics;
	using System.IO;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;
	using NUnit.Framework;

	/// <summary>
	/// Base class for test fixtures that test code builder classes.
	/// </summary>
	public class CodeBuilderTestBase
	{
		protected AssemblyName assemblyName;
		protected string assemblyFileName;
		protected string moduleName;
		protected string assemblyFilePath;
		protected AssemblyBuilder assemblyBuilder;
		protected ModuleBuilder moduleBuilder;
		private int assemblyCount;

		protected void CreateAssemblyBuilder()
		{
			assemblyName = new AssemblyName();
			assemblyName.Name = GetType().ToString() + "." + (++assemblyCount);
			moduleName = assemblyName.Name;
			string moduleFileName = moduleName + ".dll";
			assemblyFileName = moduleFileName;
			string assemblyDirectory = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			assemblyFilePath = Path.Combine(assemblyDirectory, moduleFileName);
			assemblyBuilder =
				Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.RunAndSave);
			moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, moduleFileName);
		}

		protected void VerifyAssembly()
		{
		    string[] possiblePaths = new[]
		                                 {
                                             @"c:\program files (x86)\Microsoft SDKs\Windows\v7.0A\bin\netfx 4.0 tools\peverify.exe",
                                             @"c:\program files\Microsoft SDKs\Windows\v7.0A\bin\netfx 4.0 tools\peverify.exe",
                                             @"c:\program files (x86)\Microsoft SDKs\Windows\v7.0A\bin\peverify.exe",
                                             @"c:\program files\Microsoft SDKs\Windows\v7.0A\bin\peverify.exe",
		                                 };
		    string filePath = null;
            foreach (var possiblePath in possiblePaths)
            {
                if (File.Exists(possiblePath))
                {
                    filePath = possiblePath;
                    break;
                }
            }

            if (filePath == null)
            {
                throw new NotSupportedException("Cannot find PEVERIFY.EXE on this machine");
            }

			assemblyBuilder.Save(assemblyFileName);
			Process process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.FileName = filePath;
			process.StartInfo.Arguments = "peverify.exe \"" + assemblyFileName + "\""; // " /quiet";
			process.Start();
			process.WaitForExit();
			Assert.AreEqual(0, process.ExitCode, "PEVerify failed.");
		}

		protected TInterface CreateProxy<TInterface, TInner>(Type proxyType, TInner inner)
		{
			Type[] parameterTypes = new Type[] {typeof(TInner) };
			ConstructorInfo constructor = proxyType.GetConstructor(parameterTypes);
			Assert.IsNotNull(constructor, "Cannot find constructor for proxy type");
			return (TInterface)constructor.Invoke(new object[] {inner});
		}
	}
}