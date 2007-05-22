#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2007 John Jeffery. All rights reserved.
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
	using System.Reflection;
	using System.Reflection.Emit;

	internal class NavigatorProxyBuilder
	{
		private readonly ModuleBuilder _moduleBuilder;
		private readonly Type _interfaceType;
		private readonly Type _innerType;
		private readonly string _className;

		private TypeBuilder _typeBuilder;
		private FieldBuilder _innerFieldBuilder;
		private Type _proxyType;
		private MethodInfo _navigateMethod;

		public NavigatorProxyBuilder(ModuleBuilder moduleBuilder, string className, Type interfaceType, Type innerType)
		{
			_moduleBuilder = moduleBuilder;
			_className = className;
			_interfaceType = interfaceType;
			_innerType = innerType;
		}

		public Type CreateType()
		{
			if (_proxyType != null) {
				return _proxyType;
			}

			FindNavigateMethod();

			_typeBuilder = _moduleBuilder.DefineType(
					_className,
					TypeAttributes.Public |
					TypeAttributes.Class |
					TypeAttributes.AutoClass |
					TypeAttributes.AnsiClass |
					TypeAttributes.BeforeFieldInit |
					TypeAttributes.AutoLayout,
					typeof(object),
					new Type[] {_interfaceType});

			_innerFieldBuilder = _typeBuilder.DefineField("inner", _innerType, FieldAttributes.Private);

			BuildConstructor();

			foreach (PropertyInfo property in _interfaceType.GetProperties()) {
				BuildProperty(property);
			}

			foreach (EventInfo eventInfo in _interfaceType.GetEvents()) {
				BuildEvent(eventInfo);
			}

			foreach (MethodInfo method in _interfaceType.GetMethods()) {
				BuildMethod(method);
			}

			_proxyType = _typeBuilder.CreateType();
			return _proxyType;
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
					= _typeBuilder.DefineConstructor(methodAttributes,
					                                 CallingConventions.Standard,
					                                 new Type[] {_innerType});

			ILGenerator generator = constructorBuilder.GetILGenerator();
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Call, objectConstructor);
			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldarg_1);
			generator.Emit(OpCodes.Stfld, _innerFieldBuilder);
			generator.Emit(OpCodes.Ret);
		}

		private MethodBuilder BuildMethod(MethodInfo method)
		{
			if (method.ReturnType != typeof(void)) {
				throw new ArgumentException("Navigate methods must have a return type of System.Void");
			}
			// remove the abstract flag from the method attributes as it is being implemented
			MethodAttributes methodAttributes
					= MethodAttributes.HideBySig
					  | MethodAttributes.Public
					  | MethodAttributes.NewSlot
					  | MethodAttributes.Virtual
					  | MethodAttributes.Final
					;

			ParameterInfo[] parameters = method.GetParameters();
			if (parameters != null && parameters.Length > 0) {
				throw new ArgumentException("Navigate methods must have no parameters");
			}
			Type[] parameterTypes = new Type[0];
			MethodBuilder methodBuilder = _typeBuilder.DefineMethod(
					method.Name,
					methodAttributes,
					method.CallingConvention,
					method.ReturnType,
					parameterTypes);

			ILGenerator generator = methodBuilder.GetILGenerator();

			generator.Emit(OpCodes.Ldarg_0);
			generator.Emit(OpCodes.Ldfld, _innerFieldBuilder);
			generator.Emit(OpCodes.Ldstr, method.Name);
			generator.Emit(OpCodes.Callvirt, _navigateMethod);
			generator.Emit(OpCodes.Ret);

			_typeBuilder.DefineMethodOverride(methodBuilder, method);

			return methodBuilder;
		}

		private void BuildProperty(PropertyInfo property)
		{
			throw new ArgumentException("Navigation interfaces cannot contain properties: " + property);
		}

		private void BuildEvent(EventInfo @event)
		{
			throw new ArgumentException("Navigation interfaces cannot contain events: " + @event);
		}

		private void FindNavigateMethod()
		{
			_navigateMethod = FindNavigateMethod(_innerType);
			if (_navigateMethod != null) {
				return;
			}

			foreach (Type type in _innerType.GetInterfaces()) {
				_navigateMethod = FindNavigateMethod(type);
				if (_navigateMethod != null) {
					return;
				}
			}

			throw new ArgumentException("Cannot find navigate method with signature 'void Navigate(string)'");
		}

		private static MethodInfo FindNavigateMethod(Type type)
		{
			Type[] parameterTypes = new Type[] {typeof(string)};
			MethodInfo navigateMethod = type.GetMethod("Navigate", parameterTypes);
			if (navigateMethod != null 
				&& navigateMethod.IsPublic 
				&& navigateMethod.DeclaringType.IsPublic
				&& navigateMethod.ReturnType == typeof(void)) {
				return navigateMethod;
			}
			return null;
		}
	}
}