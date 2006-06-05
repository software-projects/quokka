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
    internal class DuckProxyBuilder
    {
        private readonly Type m_interfaceType;
        private readonly Type m_innerType;
        private readonly string m_className;
        private readonly ModuleBuilder m_moduleBuilder;

        private TypeBuilder m_typeBuilder;
        private FieldBuilder m_innerFieldBuilder;
        private Type m_proxyType = null;

        // Maps a MethodInfo in the interface onto a MethodBuilder used to 
        // construct the associated method in the proxy
        private IDictionary<MethodInfo, MethodBuilder> m_interfaceMethodDict = new Dictionary<MethodInfo, MethodBuilder>();

        public DuckProxyBuilder(ModuleBuilder moduleBuilder, string className, Type interfaceType, Type innerType) {
            if (moduleBuilder == null)
                throw new ArgumentNullException();
            if (className == null)
                throw new ArgumentNullException();
            if (interfaceType == null)
                throw new ArgumentNullException();
            if (innerType == null)
                throw new ArgumentNullException();

            this.m_moduleBuilder = moduleBuilder;
            this.m_className = className;
            this.m_interfaceType = interfaceType;
            this.m_innerType = innerType;
        }

        public Type CreateType() {
            if (m_proxyType == null) {
                m_typeBuilder = m_moduleBuilder.DefineType(m_className,
                    TypeAttributes.Public |
                    TypeAttributes.Class |
                    TypeAttributes.AutoClass |
                    TypeAttributes.AnsiClass |
                    TypeAttributes.BeforeFieldInit |
                    TypeAttributes.AutoLayout,
                    typeof(object),
                    new Type[] { m_interfaceType });
                //m_typeBuilder.AddInterfaceImplementation(m_interfaceType);

                m_innerFieldBuilder = m_typeBuilder.DefineField("inner", m_innerType, FieldAttributes.Private);

                BuildConstructor();

                foreach (MethodInfo method in m_interfaceType.GetMethods()) {
                    BuildMethod(method);
                }

                foreach (PropertyInfo property in m_interfaceType.GetProperties()) {
                    BuildProperty(property);
                }

                foreach (EventInfo eventInfo in m_interfaceType.GetEvents()) {
                    BuildEvent(eventInfo);
                }

                m_proxyType = m_typeBuilder.CreateType();
            }
            return m_proxyType;
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
                = m_typeBuilder.DefineConstructor(methodAttributes,
                CallingConventions.Standard,
                new Type[] { m_innerType });

            ILGenerator generator = constructorBuilder.GetILGenerator();
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Call, objectConstructor);
            generator.Emit(OpCodes.Ldarg_0);
            generator.Emit(OpCodes.Ldarg_1);
            generator.Emit(OpCodes.Stfld, m_innerFieldBuilder);
            generator.Emit(OpCodes.Ret);
        }

        private void BuildProperty(PropertyInfo property) {
            PropertyBuilder propertyBuilder = m_typeBuilder.DefineProperty(
                property.Name,
                property.Attributes,
                property.PropertyType,
                new Type[] { property.PropertyType });

            MethodInfo getMethod = property.GetGetMethod();
            if (getMethod != null) {
                MethodBuilder getMethodBuilder = this.m_interfaceMethodDict[getMethod];
                propertyBuilder.SetGetMethod(getMethodBuilder);
            }

            MethodInfo setMethod = property.GetSetMethod();
            if (setMethod != null) {
                MethodBuilder setMethodBuilder = this.m_interfaceMethodDict[setMethod];
                propertyBuilder.SetSetMethod(setMethodBuilder);
            }
        }

        private void BuildEvent(EventInfo eventInfo) {
            EventBuilder eventBuilder = m_typeBuilder.DefineEvent(
                eventInfo.Name,
                eventInfo.Attributes,
                eventInfo.EventHandlerType);

            MethodInfo addMethod = eventInfo.GetAddMethod();
            if (addMethod != null) {
                MethodBuilder addMethodBuilder = this.m_interfaceMethodDict[addMethod];
                eventBuilder.SetAddOnMethod(addMethodBuilder);
            }

            MethodInfo removeMethod = eventInfo.GetRemoveMethod();
            if (removeMethod != null) {
                MethodBuilder removeMethodBuilder = this.m_interfaceMethodDict[removeMethod];
                eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
            }

            MethodInfo raiseMethod = eventInfo.GetRaiseMethod();
            if (raiseMethod != null) {
                MethodBuilder raiseMethodBuilder = m_interfaceMethodDict[raiseMethod];
                eventBuilder.SetRaiseMethod(raiseMethodBuilder);
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
            Type[] parameterTypes = GetParameterTypes(parameters);
            MethodBuilder methodBuilder = m_typeBuilder.DefineMethod(
                method.Name,
                methodAttributes,
                method.CallingConvention,
                method.ReturnType,
                parameterTypes);

            // remember the mapping between interface method and this method for
            // wiring up the get/set methods to properties
            this.m_interfaceMethodDict.Add(method, methodBuilder);

            ILGenerator generator = methodBuilder.GetILGenerator();

            MethodInfo innerMethod = m_innerType.GetMethod(method.Name, parameterTypes);

            if (innerMethod != null) {
                if (!method.ReturnType.IsAssignableFrom(innerMethod.ReturnType)) {
                    // TODO: throw an exception, whatever
                    innerMethod = null;
                }
            }

            if (innerMethod == null) {
                BuildNotSupportedException(generator, method.Name);
            }
            else {
                generator.Emit(OpCodes.Ldarg_0);
                generator.Emit(OpCodes.Ldfld, this.m_innerFieldBuilder);

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

            m_typeBuilder.DefineMethodOverride(methodBuilder, method);

            return methodBuilder;
        }

        void BuildNotSupportedException(ILGenerator generator, string name) {
            Type exceptionType = typeof(NotSupportedException);
            // TODO: pass name as a parameter to the constructor
            ConstructorInfo constructor = exceptionType.GetConstructor(Type.EmptyTypes);
            generator.Emit(OpCodes.Newobj, constructor);
            generator.Emit(OpCodes.Throw);
        }
    }
}
