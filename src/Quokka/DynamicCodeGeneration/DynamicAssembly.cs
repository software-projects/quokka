#region Copyright Notice
/*
 * Copyright © 2005 John Jeffery
 *
 * This software is provided 'as-is', without any express or implied warranty. In no 
 * event will the authors be held liable for any damages arising from the use of this 
 * software.
 * 
 * Permission is granted to anyone to use this software for any purpose, including 
 * commercial applications, and to alter it and redistribute it freely, subject to the 
 * following restrictions:
 *
 * 1. The origin of this software must not be misrepresented; you must not claim that 
 * you wrote the original software. If you use this software in a product, an 
 * acknowledgment (see the following) in the product documentation is required.
 *
 *		Portions Copyright © 2005 John Jeffery
 *
 * 2. Altered source versions must be plainly marked as such, and must not be 
 * misrepresented as being the original software.
 *
 * 3. This notice may not be removed or altered from any source distribution.
 *
 */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Quokka.DynamicCodeGeneration
{
    internal class DynamicAssembly
    {
        private Type interfaceType;
        private Type innerType;
        private static int assemblyCount = 0;
        private static int classCount = 0;

        private TypeBuilder typeBuilder;
        private FieldBuilder innerFieldBuilder;

        // maps a MethodInfo in the interface onto a MethodBuilder used to 
        // construct the associated method in the wrapper
        // key=MethodInfo, value=MethodBuilder
        private IDictionary interfaceMethodDict = new Hashtable();

        public DynamicAssembly(Type interfaceType, Type innerType) {
            this.interfaceType = interfaceType;
            this.innerType = innerType;
        }

        public Type Build() {
            int assemblyNumber = Interlocked.Increment(ref assemblyCount);
            AssemblyName assemblyName = new AssemblyName();
            assemblyName.Name = String.Format("Quokka.Dynamic{0}", assemblyNumber);
            string moduleName = assemblyName.Name;

#if DEBUG
            string moduleFileName = moduleName + ".dll";
            AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
                assemblyName, AssemblyBuilderAccess.RunAndSave);

            // Add a debuggable attribute to the assembly saying to disable optimizations
            // See http://blogs.msdn.com/rmbyers/archive/2005/06/26/432922.aspx
            Type daType = typeof(DebuggableAttribute);
            ConstructorInfo daCtor = daType.GetConstructor(new Type[] { typeof(bool), typeof(bool) });
            CustomAttributeBuilder daBuilder = new CustomAttributeBuilder(daCtor, new object[] { true, true });
            assemblyBuilder.SetCustomAttribute(daBuilder);

            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName, moduleFileName);
#else
				AssemblyBuilder assemblyBuilder = Thread.GetDomain().DefineDynamicAssembly(
					assemblyName, AssemblyBuilderAccess.Run);

                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule(moduleName);
#endif

            Type wrapperType = BuildType(moduleBuilder);
#if DEBUG
            assemblyBuilder.Save(moduleFileName);
#endif

            return wrapperType;
        }

        private Type BuildType(ModuleBuilder moduleBuilder) {
            int classNumber = Interlocked.Increment(ref classCount);
            string className = "Quokka.Dynamic.Proxy" + classNumber.ToString();
            typeBuilder = moduleBuilder.DefineType(className,
                TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout,
                typeof(object),
                new Type[] { interfaceType });
            //typeBuilder.AddInterfaceImplementation(interfaceType);

            innerFieldBuilder = typeBuilder.DefineField("inner", innerType, FieldAttributes.Private);

            BuildConstructor();

            foreach (MethodInfo method in interfaceType.GetMethods()) {
                BuildMethod(method);
            }

            foreach (PropertyInfo property in interfaceType.GetProperties()) {
                BuildProperty(property);
            }

            return typeBuilder.CreateType();
        }

        private void BuildConstructor() {
            Type objectType = typeof(object);
            ConstructorInfo objectConstructor = objectType.GetConstructor(Type.EmptyTypes);
            MethodAttributes methodAttributes
                = MethodAttributes.Public
                | MethodAttributes.SpecialName
                | MethodAttributes.RTSpecialName
                | MethodAttributes.HideBySig
                ;

            ConstructorBuilder constructorBuilder
                = typeBuilder.DefineConstructor(methodAttributes,
                CallingConventions.Standard,
                new Type[] { innerType });

            ILGenerator generator = constructorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, objectConstructor);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, innerFieldBuilder);
            generator.Emit(OpCodes.Ret);
        }

        private void BuildProperty(PropertyInfo property) {
            PropertyBuilder propertyBuilder = typeBuilder.DefineProperty(
                property.Name,
                property.Attributes,
                property.PropertyType,
                new Type[] { property.PropertyType });

            MethodInfo getMethod = property.GetGetMethod();
            if (getMethod != null) {
                MethodBuilder getMethodBuilder = (MethodBuilder)this.interfaceMethodDict[getMethod];
                propertyBuilder.SetGetMethod(getMethodBuilder);
            }

            MethodInfo setMethod = property.GetSetMethod();
            if (setMethod != null) {
                MethodBuilder setMethodBuilder = (MethodBuilder)this.interfaceMethodDict[setMethod];
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }
        }

        private static Type[] GetParameterTypes(ParameterInfo[] parameters) {
            Type[] types = new Type[parameters.Length];
            for (int i = 0; i < parameters.Length; ++i) {
                types[i] = parameters[i].ParameterType;
            }
            return types;
        }

        private MethodBuilder BuildMethod(MethodInfo method) {
            // remove the abstract flag from the method attributes as it is being implemented
            MethodAttributes methodAttributes
                = MethodAttributes.HideBySig
                | MethodAttributes.Public
                | MethodAttributes.NewSlot
                | MethodAttributes.Virtual
                | MethodAttributes.Final
                ;

            ParameterInfo[] parameters = method.GetParameters();
            Type[] parameterTypes = GetParameterTypes(method.GetParameters());
            MethodBuilder methodBuilder = typeBuilder.DefineMethod(
                method.Name,
                methodAttributes,
                method.CallingConvention,
                method.ReturnType,
                parameterTypes);

            // remember the mapping between interface method and this method for
            // wiring up the get/set methods to properties
            this.interfaceMethodDict.Add(method, methodBuilder);

            ILGenerator generator = methodBuilder.GetILGenerator();

            MethodInfo innerMethod = innerType.GetMethod(method.Name, parameterTypes);

            if (innerMethod != null) {
                if (!method.ReturnType.IsAssignableFrom(innerMethod.ReturnType)) {
                    // TODO: throw an exception, whatever
                    innerMethod = null;
                }
            }

            if (innerMethod == null) {
                BuildNotSupportedException(generator);
            }
            else {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, this.innerFieldBuilder);

                if (parameters.Length >= 1) {
                    generator.Emit(OpCodes.Ldarg_1);
                    if (parameters.Length >= 2) {
                        generator.Emit(OpCodes.Ldarg_2);
                    }
                    if (parameters.Length >= 3) {
                        generator.Emit(OpCodes.Ldarg_3);
                    }
                    for (byte index = 3; index < parameters.Length; ++index) {
                        generator.Emit(OpCodes.Ldarg_S, index + 1);
                    }
                }

                generator.Emit(OpCodes.Callvirt, innerMethod);
                generator.Emit(OpCodes.Ret);
            }

            typeBuilder.DefineMethodOverride(methodBuilder, method);

            return methodBuilder;
        }

        void BuildNotSupportedException(ILGenerator generator) {
            Type exceptionType = typeof(NotSupportedException);
            ConstructorInfo constructor = exceptionType.GetConstructor(Type.EmptyTypes);
            generator.Emit(OpCodes.Newobj, constructor);
            generator.Emit(OpCodes.Throw);
        }
    }
}
