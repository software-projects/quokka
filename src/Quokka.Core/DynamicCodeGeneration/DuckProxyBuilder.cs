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

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace Quokka.DynamicCodeGeneration
{
    internal class DuckProxyBuilder
	{
		private readonly Type m_interfaceType;
		private readonly Type m_innerType;
		private readonly string m_className;
		private readonly ModuleBuilder m_moduleBuilder;
        private readonly Dictionary<MethodInfo, MethodBuilder> _unsupportedMethods = new Dictionary<MethodInfo, MethodBuilder>();

		private TypeBuilder m_typeBuilder;
		private FieldBuilder m_innerFieldBuilder;
		private Type m_proxyType = null;

		// Maps a MethodInfo in the interface onto a MethodBuilder used to 
		// construct the associated method in the proxy
		private readonly IDictionary<MethodInfo, MethodBuilder> m_interfaceMethodDict = new Dictionary<MethodInfo, MethodBuilder>();

		internal DuckProxyBuilder(ModuleBuilder moduleBuilder, string className, Type interfaceType, Type innerType)
		{
			m_moduleBuilder = moduleBuilder;
			m_className = className;
			m_interfaceType = interfaceType;
			m_innerType = innerType;
		}

		public Type CreateType()
		{
			if (m_proxyType == null) {
				m_typeBuilder = m_moduleBuilder.DefineType(m_className,
				                                           TypeAttributes.Public |
				                                           TypeAttributes.Class |
				                                           TypeAttributes.AutoClass |
				                                           TypeAttributes.AnsiClass |
				                                           TypeAttributes.BeforeFieldInit |
				                                           TypeAttributes.AutoLayout,
				                                           typeof(object),
				                                           new Type[] {m_interfaceType});
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

                foreach (MethodBuilder methodBuilder in _unsupportedMethods.Values)
                {
                    BuildNotSupportedException(methodBuilder);
                }

				m_proxyType = m_typeBuilder.CreateType();
			}
			return m_proxyType;
		}

		private void BuildConstructor()
		{
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
				                                  new Type[] {m_innerType});

			ILGenerator generator = constructorBuilder.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Call, objectConstructor);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Stfld, m_innerFieldBuilder);
			generator.Emit(OpCodes.Ret);
		}

		private void BuildProperty(PropertyInfo property)
		{
			PropertyBuilder propertyBuilder = m_typeBuilder.DefineProperty(
				property.Name,
				property.Attributes,
				property.PropertyType,
				GetParameterTypes(property.GetIndexParameters()));

			MethodInfo getMethod = property.GetGetMethod();
			if (getMethod != null) {
				MethodBuilder getMethodBuilder = m_interfaceMethodDict[getMethod];
                if (_unsupportedMethods.ContainsKey(getMethod) && getMethodBuilder.ReturnType == typeof(bool))
                {
                    // The property is not supported by the inner class, but it is a bool property so it
                    // might be a test for is a member supported. Using naming convention, try to extract
                    // the name of the member we are testing for.
                    string testMemberName = GetTestMemberName(property.Name);
                    if (testMemberName != null)
                    {
                        // This is a property for checking whether a member is supported.
                        _unsupportedMethods.Remove(getMethod);
                        BuildCheckMemberSupported(testMemberName, getMethodBuilder);
                    }
                }
				propertyBuilder.SetGetMethod(getMethodBuilder);
			}

			MethodInfo setMethod = property.GetSetMethod();
			if (setMethod != null) {
				MethodBuilder setMethodBuilder = m_interfaceMethodDict[setMethod];
				propertyBuilder.SetSetMethod(setMethodBuilder);
			}
		}

        private static string GetTestMemberName(string propertyName)
        {
            const string prefix1 = "Can";

            if (propertyName.StartsWith(prefix1))
            {
                return propertyName.Substring(prefix1.Length);
            }

            const string prefix2 = "Is";
            const string suffix2 = "Supported";

            if (propertyName.StartsWith(prefix2) && propertyName.EndsWith(suffix2))
            {
                return propertyName.Substring(prefix2.Length, propertyName.Length - prefix2.Length - suffix2.Length);
            }

            return null;
        }

		private void BuildEvent(EventInfo eventInfo)
		{
			EventBuilder eventBuilder = m_typeBuilder.DefineEvent(
				eventInfo.Name,
				eventInfo.Attributes,
				eventInfo.EventHandlerType);

			MethodInfo addMethod = eventInfo.GetAddMethod();
			if (addMethod != null) {
				MethodBuilder addMethodBuilder = m_interfaceMethodDict[addMethod];
				eventBuilder.SetAddOnMethod(addMethodBuilder);
			}

			MethodInfo removeMethod = eventInfo.GetRemoveMethod();
			if (removeMethod != null) {
				MethodBuilder removeMethodBuilder = m_interfaceMethodDict[removeMethod];
				eventBuilder.SetRemoveOnMethod(removeMethodBuilder);
			}

			MethodInfo raiseMethod = eventInfo.GetRaiseMethod();
			if (raiseMethod != null) {
				MethodBuilder raiseMethodBuilder = m_interfaceMethodDict[raiseMethod];
				eventBuilder.SetRaiseMethod(raiseMethodBuilder);
			}
		}

		private static Type[] GetParameterTypes(ParameterInfo[] parameters)
		{
			if (parameters == null)
				return Type.EmptyTypes;

			Type[] types = new Type[parameters.Length];
			for (int i = 0; i < parameters.Length; ++i) {
				types[i] = parameters[i].ParameterType;
			}
			return types;
		}

		private MethodBuilder BuildMethod(MethodInfo method)
		{
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
			m_interfaceMethodDict.Add(method, methodBuilder);


			MethodInfo innerMethod = m_innerType.GetMethod(method.Name, parameterTypes);

			if (innerMethod != null) {
				if (!method.ReturnType.IsAssignableFrom(innerMethod.ReturnType)) {
					// TODO: throw an exception, whatever
					innerMethod = null;
				}
			}

			if (innerMethod == null) {
				//BuildNotSupportedException(generator);
			    _unsupportedMethods.Add(method, methodBuilder);
			}
			else {
                ILGenerator generator = methodBuilder.GetILGenerator();

				generator.Emit(OpCodes.Ldarg_0);
				generator.Emit(OpCodes.Ldfld, m_innerFieldBuilder);

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

		private static void BuildNotSupportedException(ILGenerator generator)
		{
			Type exceptionType = typeof(NotSupportedException);
			// TODO: pass name as a parameter to the constructor
			ConstructorInfo constructor = exceptionType.GetConstructor(Type.EmptyTypes);
			generator.Emit(OpCodes.Newobj, constructor);
			generator.Emit(OpCodes.Throw);
		}

        private static void BuildNotSupportedException(MethodBuilder methodBuilder)
        {
            BuildNotSupportedException(methodBuilder.GetILGenerator());
        }

        private void BuildCheckMemberSupported(string memberName, MethodBuilder methodBuilder)
        {
            ILGenerator generator = methodBuilder.GetILGenerator();

            foreach (MemberInfo member in m_innerType.GetMembers())
            {
                if (member.Name == memberName)
                {
                    generator.Emit(OpCodes.Ldc_I4_1);
                    generator.Emit(OpCodes.Ret);
                    return;
                }
            }

            generator.Emit(OpCodes.Ldc_I4_0);
            generator.Emit(OpCodes.Ret);
        }
	}
}