#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006 John Jeffery. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#endregion

namespace Quokka.DynamicCodeGeneration
{
	using System;
	using System.Diagnostics;
	using System.Reflection;
	using System.Reflection.Emit;
	using System.Threading;

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
            return dynamicClassNamespace + "." + text + classNumber;
        }
    }
}