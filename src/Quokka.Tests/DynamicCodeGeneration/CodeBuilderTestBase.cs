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
			assemblyBuilder.Save(assemblyFileName);
			Process process = new Process();
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.FileName = @"c:\program files\Microsoft Visual Studio 8\SDK\v2.0\Bin\peverify.exe";
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