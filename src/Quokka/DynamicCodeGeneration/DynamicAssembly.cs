using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;

namespace Quokka.DynamicCodeGeneration
{
    internal class DynamicAssembly
    {
        // used to create a unique name for each dynamic assembly
        private static int assemblyCount = 0;
        private int classCount = 0;
        private readonly AssemblyBuilder m_assemblyBuilder;
        private readonly ModuleBuilder m_moduleBuilder;
        private readonly string dynamicClassNamespace;

        public DynamicAssembly() {
            int assemblyNumber = Interlocked.Increment(ref assemblyCount);
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = String.Format("Quokka.Dynamic.N{0}", assemblyNumber);
            string moduleName = assemblyName.Name;
            dynamicClassNamespace = assemblyName.Name;

#if DEBUG
            string moduleFileName = moduleName + ".dll";
            m_assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                assemblyName, AssemblyBuilderAccess.RunAndSave);

            // Add a debuggable attribute to the assembly saying to disable optimizations
            // See http://blogs.msdn.com/rmbyers/archive/2005/06/26/432922.aspx
            Type daType = typeof(DebuggableAttribute);
            ConstructorInfo daCtor = daType.GetConstructor(new Type[] { typeof(bool), typeof(bool) });
            CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] { true, true });
            m_assemblyBuilder.SetCustomAttribute(daBuilder);

            m_moduleBuilder = m_assemblyBuilder.DefineDynamicModule(moduleName, moduleFileName);
#else
			m_assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            m_moduleBuilder = m_assemblyBuilder.DefineDynamicModule(moduleName);
#endif
        }

        public Type CreateDuckProxyType(Type interfaceType, Type innerType) {
            string className = NewClassName("DuckProxy");
            DuckProxyBuilder builder = new DuckProxyBuilder(m_moduleBuilder, className, interfaceType, innerType);
            return builder.CreateType();
        }

        private string NewClassName(string text) {
            int classNumber = Interlocked.Increment(ref classCount);
            return dynamicClassNamespace + "." + text + classNumber.ToString();
        }
    }
}