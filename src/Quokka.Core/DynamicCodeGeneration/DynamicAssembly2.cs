using System;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Quokka.DynamicCodeGeneration
{
	/// <summary>
	/// Provides a central reference to an assembly that is created dynamically during program execution.
	/// A <see cref="DynamicAssembly"/> is not normally written to disk, but it can be for testing purposes.
	/// </summary>
	public class DynamicAssembly
	{
		// used to create a unique name for each dynamic assembly
		private static int _assemblyCount;
		private readonly AssemblyName _assemblyName;
		private readonly AssemblyBuilder _assemblyBuilder;
		private readonly string _dynamicClassNamespace;
		private readonly ModuleBuilder _moduleBuilder;
		private readonly bool _canSave;
		private int _classCount;

		private static DynamicAssembly _instance;
		private static readonly object LockObject = new object();

		public static DynamicAssembly Instance
		{
			get
			{
				if (_instance != null)
				{
					return _instance;
				}

				lock (LockObject)
				{
					if (_instance == null)
					{
						_instance = new DynamicAssembly();
					}
				}
				return _instance;
			}
		}

		public DynamicAssembly()
		{
			int assemblyNumber = Interlocked.Increment(ref _assemblyCount);
			_assemblyName = new AssemblyName
			                	{
			                		Name = String.Format("Quokka.DynamicAssembly.N{0}", assemblyNumber)
			                	};
			string moduleName = AssemblyName.Name;
			_dynamicClassNamespace = AssemblyName.Name;

			if (CreateFiles)
			{
				_assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
					AssemblyName, AssemblyBuilderAccess.RunAndSave);

				// Add a debuggable attribute to the assembly saying to disable optimizations
				// See http://blogs.msdn.com/rmbyers/archive/2005/06/26/432922.aspx
				Type daType = typeof(DebuggableAttribute);
				ConstructorInfo daCtor = daType.GetConstructor(new[] { typeof(bool), typeof(bool) });
				var daBuilder = new CustomAttributeBuilder(daCtor, new object[] { true, true });
				_assemblyBuilder.SetCustomAttribute(daBuilder);

				_moduleBuilder = _assemblyBuilder.DefineDynamicModule(moduleName);
				_canSave = true;
			}
			else
			{
				_assemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(AssemblyName, AssemblyBuilderAccess.Run);
				_moduleBuilder = _assemblyBuilder.DefineDynamicModule(moduleName);
			}
		}

		/// <summary>
		/// If set to <c>true</c>, then any <see cref="DynamicAssembly"/> objects created will be able to be saved to disk.
		/// If set to <c>false</c>, then <see cref="DynamicAssembly"/> objects cannot be saved to disk.
		/// </summary>
		/// <remarks>
		/// The main reason for wanting to save a <see cref="DynamicAssembly"/> object to disk is so that it can
		/// be verified by the PEVerify utility as part of unit testing.
		/// </remarks>
		public static bool CreateFiles { get; set; }

		public AssemblyName AssemblyName
		{
			get { return _assemblyName; }
		}

		public ModuleBuilder ModuleBuilder
		{
			get { return _moduleBuilder; }
		}

		public bool CanSave
		{
			get { return _canSave; }
		}

		public void Save(string filePath)
		{
			// TODO: throw invalid operation exception if !CanSave
			_assemblyBuilder.Save(filePath);
		}

		public void Save()
		{
			// TODO: throw invalid operation exception if !CanSave
			_assemblyBuilder.Save(AssemblyName.Name + ".dll");
		}

		public TypeBuilder DefineType(string text, TypeAttributes attr, Type parent, Type[] interfaces)
		{
			string className = CreateClassName(text);
			return _moduleBuilder.DefineType(className, attr, parent, interfaces);
		}

		public TypeBuilder DefineType(string text, TypeAttributes attr, Type parent)
		{
			string className = CreateClassName(text);
			return _moduleBuilder.DefineType(className, attr, parent);
		}

		public string CreateClassName(string text)
		{
			int classNumber = Interlocked.Increment(ref _classCount);
			return _dynamicClassNamespace + "." + text + classNumber;
		}
	}
}