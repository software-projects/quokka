//#region Copyright notice
//
////
//// Authors: 
////  John Jeffery <john@jeffery.id.au>
////
//// Copyright (C) 2007 John Jeffery. All rights reserved.
////
//// Permission is hereby granted, free of charge, to any person obtaining
//// a copy of this software and associated documentation files (the
//// "Software"), to deal in the Software without restriction, including
//// without limitation the rights to use, copy, modify, merge, publish,
//// distribute, sublicense, and/or sell copies of the Software, and to
//// permit persons to whom the Software is furnished to do so, subject to
//// the following conditions:
//// 
//// The above copyright notice and this permission notice shall be
//// included in all copies or substantial portions of the Software.
//// 
//// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
//// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
//// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
//// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
//// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
//// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
////
//
//#endregion
//
//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using System.Reflection.Emit;
//using Quokka.Diagnostics;
//using Quokka.Uip;
//
//namespace Quokka.DynamicCodeGeneration
//{
//    public class NavigatorProxyBuilder : AbstractProxyBuilder
//	{
//		private const string CanNavigatePropertyPrefix = "CanNavigate";
//		private const string CanNavigateMethodName = "CanNavigate";
//		private const string NavigateMethodName = "Navigate";
//
//		private readonly MethodInfo _innerNavigateMethod;
//		private readonly MethodInfo _innerCanNavigateMethod;
//		private readonly MethodInfo _outerNavigateEnumMethod;
//		private readonly MethodInfo _outerCanNavigateEnumMethod;
//		private readonly Type _navigateValueEnumType;
//		private readonly List<string> _navigateValues;
//		private readonly List<MethodInfo> _outerNavigateMethods;
//		private readonly List<PropertyInfo> _outerCanNavigateProperties;
//		private readonly List<MethodInfo> _dodgyMethods = new List<MethodInfo>();
//
//		public NavigatorProxyBuilder(ModuleBuilder moduleBuilder, string className, Type interfaceType, Type innerType)
//			: base(moduleBuilder, className, interfaceType, innerType)
//		{
//			_navigateValues = new List<string>();
//
//			_innerNavigateMethod = FindInnerNavigateMethod(innerType);
//			if (_innerNavigateMethod == null)
//			{
//				AddErrorMessage("Cannot find method with signature 'void Navigate(string)' in type {0}", innerType);
//			}
//
//			_innerCanNavigateMethod = FindInnerCanNavigateMethod(innerType);
//			if (_innerCanNavigateMethod == null)
//			{
//				AddErrorMessage("Cannot find method with signature 'bool CanNavigate(string)' in type {0}", innerType);
//			}
//
//			_outerNavigateEnumMethod = FindOuterNavigateEnumMethod(interfaceType);
//			if (_outerNavigateEnumMethod != null)
//			{
//				_navigateValueEnumType = _outerNavigateEnumMethod.GetParameters()[0].ParameterType;
//				_outerCanNavigateEnumMethod = FindOuterCanNavigateEnumMethod(interfaceType, _navigateValueEnumType);
//				_navigateValues.AddRange(Enum.GetNames(_navigateValueEnumType));
//			}
//
//			FindEvents();
//			_outerNavigateMethods = FindOuterNavigateMethods();
//			_outerCanNavigateProperties = FindOuterCanNavigateProperties();
//		}
//
//		protected override void DoCreateType()
//		{
//			BuildConstructor();
//			BuildOuterNavigateEnumMethod();
//			BuildOuterCanNavigateEnumMethod();
//			BuildOuterNavigateMethods();
//			BuildOuterCanNavigateProperties();
//		}
//
//		private static MethodInfo FindInnerNavigateMethod(Type type)
//		{
//			Type[] parameterTypes = new Type[] { typeof(string) };
//			MethodInfo navigateMethod = type.GetMethod(NavigateMethodName, parameterTypes);
//			if (navigateMethod != null
//				&& navigateMethod.IsPublic
//				&& navigateMethod.DeclaringType.IsPublic
//				&& navigateMethod.ReturnType == typeof(void))
//			{
//				return navigateMethod;
//			}
//
//			// At this point the type does not have a matching method, but check its
//			// interfaces for a matching method.
//			if (!type.IsInterface)
//			{
//				foreach (Type interfaceType in type.GetInterfaces())
//				{
//					navigateMethod = FindInnerNavigateMethod(interfaceType);
//					if (navigateMethod != null)
//					{
//						return navigateMethod;
//					}
//				}
//			}
//
//			return null;
//		}
//
//		private static MethodInfo FindInnerCanNavigateMethod(Type type)
//		{
//			Type[] parameterTypes = new Type[] { typeof(string) };
//			MethodInfo canNavigateMethod = type.GetMethod(CanNavigateMethodName, parameterTypes);
//			if (canNavigateMethod != null
//				&& canNavigateMethod.IsPublic
//				&& canNavigateMethod.DeclaringType.IsPublic
//				&& canNavigateMethod.ReturnType == typeof(bool))
//			{
//				return canNavigateMethod;
//			}
//
//			// At this point the type does not have a matching method, but check its
//			// interfaces for a matching method.
//			if (!type.IsInterface)
//			{
//				foreach (Type interfaceType in type.GetInterfaces())
//				{
//					canNavigateMethod = FindInnerCanNavigateMethod(interfaceType);
//					if (canNavigateMethod != null)
//					{
//						return canNavigateMethod;
//					}
//				}
//			}
//
//			return null;
//		}
//
//		private MethodInfo FindOuterNavigateEnumMethod(Type type)
//		{
//			foreach (MethodInfo method in type.GetMethods())
//			{
//				if (method.Name == NavigateMethodName)
//				{
//					bool error = false;
//
//					if (method.ReturnType != typeof(void))
//					{
//						AddErrorMessage("Method '{0}' should have a return type of void", method.Name);
//						_dodgyMethods.Add(method);
//						error = true;
//					}
//
//					ParameterInfo[] parameters = method.GetParameters();
//					if (parameters == null || parameters.Length != 1 || !parameters[0].ParameterType.IsEnum)
//					{
//						AddErrorMessage("Method '{0}' should have one parameter which is an enumerated type", method.Name);
//						_dodgyMethods.Add(method);
//						error = true;
//					}
//
//					if (!error)
//					{
//						return method;
//					}
//				}
//			}
//
//			return null;
//		}
//
//		private static MethodInfo FindOuterCanNavigateEnumMethod(Type type, Type parameterType)
//		{
//			foreach (MethodInfo method in type.GetMethods())
//			{
//				if (method.ReturnType != typeof(bool))
//				{
//					continue;
//				}
//
//				if (method.Name != CanNavigateMethodName)
//				{
//					continue;
//				}
//
//				ParameterInfo[] parameters = method.GetParameters();
//				if (parameters == null || parameters.Length != 1)
//				{
//					continue;
//				}
//
//				ParameterInfo parameter = parameters[0];
//				if (parameter.ParameterType != parameterType)
//				{
//					continue;
//				}
//
//				// found our method
//				return method;
//			}
//
//			return null;
//		}
//
//		private void FindEvents()
//		{
//			foreach (EventInfo eventInfo in InterfaceType.GetEvents())
//			{
//				AddErrorMessage("Event {0}: events are not supported in navigator interfaces", eventInfo.Name);
//				_dodgyMethods.Add(eventInfo.GetAddMethod());
//				_dodgyMethods.Add(eventInfo.GetRemoveMethod());
//				_dodgyMethods.Add(eventInfo.GetRaiseMethod());
//				_dodgyMethods.AddRange(eventInfo.GetOtherMethods());
//			}
//		}
//
//		private List<MethodInfo> FindOuterNavigateMethods()
//		{
//			// get all of the public methods
//			List<MethodInfo> methods = new List<MethodInfo>(InterfaceType.GetMethods());
//
//			// remove any property set/get methods
//			foreach (PropertyInfo property in InterfaceType.GetProperties())
//			{
//				MethodInfo getMethod = property.GetGetMethod();
//				if (getMethod != null)
//				{
//					methods.Remove(getMethod);
//				}
//				MethodInfo setMethod = property.GetSetMethod();
//				if (setMethod != null)
//				{
//					methods.Remove(setMethod);
//				}
//			}
//
//			foreach (MethodInfo method in _dodgyMethods)
//			{
//				if (method != null)
//				{
//					methods.Remove(method);
//				}
//			}
//
//			// remove the known methods
//			methods.Remove(_outerCanNavigateEnumMethod);
//			methods.Remove(_outerNavigateEnumMethod);
//
//			foreach (MethodInfo method in methods)
//			{
//				if (method.ReturnType != typeof(void))
//				{
//					AddErrorMessage("Method '{0}' should have a return type of void", method.Name);
//				}
//
//				if (method.GetParameters().Length != 0)
//				{
//					AddErrorMessage("Method '{0}' should have no parameters", method.Name);
//				}
//
//				if (!_navigateValues.Contains(method.Name))
//				{
//					_navigateValues.Add(method.Name);
//				}
//			}
//
//			return methods;
//		}
//
//		private List<PropertyInfo> FindOuterCanNavigateProperties()
//		{
//			List<PropertyInfo> properties = new List<PropertyInfo>(InterfaceType.GetProperties());
//
//			foreach (PropertyInfo property in properties)
//			{
//				if (!property.Name.StartsWith(CanNavigatePropertyPrefix))
//				{
//					AddErrorMessage("Property '{0}' should have a name starting with '{1}'", property.Name, CanNavigatePropertyPrefix);
//				}
//				else
//				{
//					string navigateValue = property.Name.Substring(CanNavigatePropertyPrefix.Length);
//					if (!_navigateValues.Contains(navigateValue))
//					{
//						AddErrorMessage("Property '{0}' checks for unknown navigation value '{1}'", property.Name, navigateValue);
//					}
//				}
//				if (property.PropertyType != typeof(bool))
//				{
//					AddErrorMessage("Property '{0}' should return type System.Boolean", property.Name);
//				}
//				if (property.GetSetMethod() != null)
//				{
//					AddErrorMessage("Property '{0}', should not be writeable", property.Name);
//				}
//				if (property.GetGetMethod() == null)
//				{
//					AddErrorMessage("Property '{0}' should be readable", property.Name);
//				}
//
//				ParameterInfo[] indexParameters = property.GetIndexParameters();
//				if (indexParameters != null && indexParameters.Length > 0)
//				{
//					AddErrorMessage("Property '{0}' should not have any index parameters");
//				}
//			}
//
//			return properties;
//		}
//
//		private void BuildConstructor()
//		{
//			Type objectType = typeof(object);
//			ConstructorInfo objectConstructor = objectType.GetConstructor(Type.EmptyTypes);
//			MethodAttributes methodAttributes = MethodAttributes.Public
//												| MethodAttributes.SpecialName
//												| MethodAttributes.RTSpecialName
//												| MethodAttributes.HideBySig;
//
//			ConstructorBuilder constructorBuilder = TypeBuilder.DefineConstructor(methodAttributes,
//																				  CallingConventions.Standard,
//																				  new Type[] { InnerType });
//
//			ILGenerator generator = constructorBuilder.GetILGenerator();
//			generator.Emit(OpCodes.Ldarg_0);
//			generator.Emit(OpCodes.Call, objectConstructor);
//			generator.Emit(OpCodes.Ldarg_0);
//			generator.Emit(OpCodes.Ldarg_1);
//			generator.Emit(OpCodes.Stfld, InnerFieldBuilder);
//			generator.Emit(OpCodes.Ret);
//		}
//
//		private void BuildOuterCanNavigateProperties()
//		{
//			foreach (PropertyInfo outerCanNavigateProperty in _outerCanNavigateProperties)
//			{
//				string navigateValue = outerCanNavigateProperty.Name.Substring(CanNavigatePropertyPrefix.Length);
//
//				MethodInfo outerCanNavigateMethod = outerCanNavigateProperty.GetGetMethod();
//
//				MethodAttributes methodAttributes = (outerCanNavigateMethod.Attributes | MethodAttributes.Final)
//													& (~MethodAttributes.Abstract);
//				MethodBuilder methodBuilder = TypeBuilder.DefineMethod(outerCanNavigateMethod.Name, methodAttributes,
//																	   outerCanNavigateMethod.CallingConvention,
//																	   outerCanNavigateMethod.ReturnType, Type.EmptyTypes);
//				ILGenerator generator = methodBuilder.GetILGenerator();
//
//				generator.Emit(OpCodes.Ldarg_0);
//				generator.Emit(OpCodes.Ldfld, InnerFieldBuilder);
//				generator.Emit(OpCodes.Ldstr, navigateValue);
//				generator.Emit(OpCodes.Callvirt, _innerCanNavigateMethod);
//				generator.Emit(OpCodes.Ret);
//
//				TypeBuilder.DefineMethodOverride(methodBuilder, outerCanNavigateMethod);
//
//				PropertyAttributes propertyAttributes = outerCanNavigateProperty.Attributes;
//
//				PropertyBuilder propertyBuilder = TypeBuilder.DefineProperty(outerCanNavigateProperty.Name, propertyAttributes,
//																			 outerCanNavigateProperty.PropertyType,
//																			 Type.EmptyTypes);
//
//				propertyBuilder.SetGetMethod(methodBuilder);
//			}
//		}
//
//		private void BuildOuterNavigateMethods()
//		{
//			foreach (MethodInfo outerNavigateMethod in _outerNavigateMethods)
//			{
//				MethodAttributes methodAttributes = (outerNavigateMethod.Attributes | MethodAttributes.Final)
//													& (~MethodAttributes.Abstract);
//				MethodBuilder methodBuilder = TypeBuilder.DefineMethod(outerNavigateMethod.Name, methodAttributes,
//																	   outerNavigateMethod.CallingConvention,
//																	   outerNavigateMethod.ReturnType, Type.EmptyTypes);
//				ILGenerator generator = methodBuilder.GetILGenerator();
//
//				generator.Emit(OpCodes.Ldarg_0);
//				generator.Emit(OpCodes.Ldfld, InnerFieldBuilder);
//				generator.Emit(OpCodes.Ldstr, outerNavigateMethod.Name);
//				generator.Emit(OpCodes.Callvirt, _innerNavigateMethod);
//				generator.Emit(OpCodes.Ret);
//
//				TypeBuilder.DefineMethodOverride(methodBuilder, outerNavigateMethod);
//			}
//		}
//
//		private void BuildOuterCanNavigateEnumMethod()
//		{
//			if (_outerCanNavigateEnumMethod == null)
//			{
//				return;
//			}
//
//			MethodAttributes methodAttributes = MethodAttributes.HideBySig
//												| MethodAttributes.Public
//												| MethodAttributes.NewSlot
//												| MethodAttributes.Virtual
//												| MethodAttributes.Final;
//
//			Type[] parameterTypes = new Type[] { _navigateValueEnumType };
//			MethodInfo toStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);
//			Verify.IsNotNull(toStringMethod);
//
//			MethodBuilder methodBuilder = TypeBuilder.DefineMethod(_outerCanNavigateEnumMethod.Name,
//																   methodAttributes,
//																   _outerCanNavigateEnumMethod.CallingConvention,
//																   _outerCanNavigateEnumMethod.ReturnType,
//																   parameterTypes);
//
//			ILGenerator generator = methodBuilder.GetILGenerator();
//
//			generator.Emit(OpCodes.Ldarg_0);
//			generator.Emit(OpCodes.Ldfld, InnerFieldBuilder);
//			generator.Emit(OpCodes.Ldarg_1);
//			generator.Emit(OpCodes.Box, _navigateValueEnumType);
//			generator.Emit(OpCodes.Callvirt, toStringMethod);
//			generator.Emit(OpCodes.Callvirt, _innerCanNavigateMethod);
//			generator.Emit(OpCodes.Ret);
//
//			TypeBuilder.DefineMethodOverride(methodBuilder, _outerCanNavigateEnumMethod);
//		}
//
//		private void BuildOuterNavigateEnumMethod()
//		{
//			if (_outerNavigateEnumMethod == null)
//			{
//				return;
//			}
//
//			// remove the abstract flag from the method attributes as it is being implemented
//			MethodAttributes methodAttributes = MethodAttributes.HideBySig
//												| MethodAttributes.Public
//												| MethodAttributes.NewSlot
//												| MethodAttributes.Virtual
//												| MethodAttributes.Final;
//
//			Type[] parameterTypes = new Type[] { _navigateValueEnumType };
//
//			MethodInfo toStringMethod = typeof(object).GetMethod("ToString", Type.EmptyTypes);
//			Verify.IsNotNull(toStringMethod);
//
//			MethodBuilder methodBuilder = TypeBuilder.DefineMethod(_outerNavigateEnumMethod.Name,
//																   methodAttributes,
//																   _outerNavigateEnumMethod.CallingConvention,
//																   _outerNavigateEnumMethod.ReturnType,
//																   parameterTypes);
//
//			ILGenerator generator = methodBuilder.GetILGenerator();
//
//			generator.Emit(OpCodes.Ldarg_0);
//			generator.Emit(OpCodes.Ldfld, InnerFieldBuilder);
//			generator.Emit(OpCodes.Ldarg_1);
//			generator.Emit(OpCodes.Box, _navigateValueEnumType);
//			generator.Emit(OpCodes.Callvirt, toStringMethod);
//			generator.Emit(OpCodes.Callvirt, _innerNavigateMethod);
//			generator.Emit(OpCodes.Ret);
//
//			TypeBuilder.DefineMethodOverride(methodBuilder, _outerNavigateEnumMethod);
//		}
//	}
//
//	public enum NavigateValue
//	{
//		OneValue,
//		AnotherValue,
//	}
//
//	public interface INavigateExample
//	{
//		void Navigate(NavigateValue navigateValue);
//		bool CanNavigate(NavigateValue navigateValue);
//
//		bool CanNavigateOneValue { get; }
//		void OneValue();
//	}
//
//	public class NavigateProxy : INavigateExample
//	{
//		private IUipNavigator inner;
//
//		public NavigateProxy(IUipNavigator inner)
//		{
//			this.inner = inner;
//		}
//
//		public void Navigate(NavigateValue navigateValue)
//		{
//			inner.Navigate(navigateValue.ToString());
//		}
//
//		public bool CanNavigate(NavigateValue navigateValue)
//		{
//			return inner.CanNavigate(navigateValue.ToString());
//		}
//
//		public bool CanNavigateOneValue
//		{
//			get { return inner.CanNavigate("OneValue"); }
//		}
//
//		public void OneValue()
//		{
//			inner.Navigate("OneValue");
//		}
//
//	    public bool IsOneValueSupported
//	    {
//            get { return true; }
//	    }
//	}
//}