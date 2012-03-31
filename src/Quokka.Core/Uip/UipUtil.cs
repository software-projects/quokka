//#region Copyright notice
//
////
//// Authors: 
////  John Jeffery <john@jeffery.id.au>
////
//// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
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
//using System.Reflection;
//using Quokka.DynamicCodeGeneration;
//
//namespace Quokka.Uip
//{
//	[Obsolete("This will be removed from Quokka in a future release")]
//	public static class UipUtil
//	{
//		/// <summary>
//		/// Assign a controller to a view.
//		/// </summary>
//		/// <param name="view">The view object</param>
//		/// <param name="controller">The controller to assign</param>
//		/// <param name="throwOnError">Throw an exception if the <c>SetController</c> method cannot be found.</param>
//		/// <remarks>
//		/// <para>
//		/// This method assigns a controller to a view. It looks for a public
//		/// method called <c>SetController</c> that takes one parameter, which
//		/// is the controller.
//		/// </para>
//		/// <para>
//		/// If the <c>SetController</c> method requires an interface, and the
//		/// controller does not directly implement that interface, then this 
//		/// method will attempt to create a 'Duck Proxy'.
//		/// </para>
//		/// </remarks>
//		public static bool SetController(object view, object controller, bool throwOnError)
//		{
//			bool result = false;
//
//			if (SetMethod(view, "SetController", ProxyType.DuckProxy, controller))
//			{
//				result = true;
//			}
//
//			if (SetProperty(view, "Controller", ProxyType.DuckProxy, controller))
//			{
//				result = true;
//			}
//
//			if (!result && throwOnError)
//			{
//				throw new QuokkaException("Cannot set controller using SetController method or Controller property");
//			}
//
//			return result;
//		}
//
//        public static bool SetView(object controller, object view, bool throwOnError)
//        {
//            bool result = false;
//
//            if (SetMethod(controller, "SetView", ProxyType.DuckProxy, view))
//            {
//                result = true;
//            }
//
//            else if (SetProperty(controller, "View", ProxyType.DuckProxy, view))
//            {
//                result = true;
//            }
//
//            if (!result && throwOnError)
//            {
//                throw new QuokkaException("Cannot set view using SetView method or View property");
//            }
//
//            return result;
//        }
//
//		public static bool SetNavigator(object target, object navigator)
//		{
//			bool result = false;
//
//			if (SetMethod(target, "SetNavigator", ProxyType.NavigatorProxy, navigator))
//			{
//				result = true;
//			}
//
//			if (SetProperty(target, "Navigator", ProxyType.NavigatorProxy, navigator))
//			{
//				result = true;
//			}
//
//			return result;
//		}
//
//		public static bool SetViewManager(object target, object viewManager)
//		{
//			bool result = false;
//
//			if (SetMethod(target, "SetViewManager", ProxyType.DuckProxy, viewManager)) {
//				result = true;
//			}
//
//			if (SetProperty(target, "ViewManager", ProxyType.DuckProxy, viewManager)) {
//				result = true;
//			}
//
//			return result;
//		}
//
//		/// <summary>
//		/// Assign a state to a view or a controller.
//		/// </summary>
//		/// <param name="obj">The view or controller object</param>
//		/// <param name="state">The state object to assign</param>
//		/// <param name="throwOnError">Throw an error if the <c>SetState</c> method cannot be found.</param>
//		/// <remarks>
//		/// <para>
//		/// This method assigns a state object to a view or controller. It looks for a public
//		/// method called <c>SetState</c> that takes one parameter, which
//		/// is the state.
//		/// </para>
//		/// <para>
//		/// If the <c>SetState</c> method requires an interface, and the
//		/// controller does not directly implement that interface, then this 
//		/// method will attempt to create a 'Duck Proxy'.
//		/// </para>
//		/// </remarks>
//		public static bool SetState(object obj, object state, bool throwOnError) {
//			bool result = false;
//
//			if (SetMethod(obj, "SetState", ProxyType.DuckProxy, state)) {
//				result = true;
//			}
//
//			if (SetProperty(obj, "State", ProxyType.DuckProxy, state)) {
//				result = true;
//			}
//
//			if (!result && throwOnError) {
//				throw new QuokkaException("Cannot set state via SetState method or State property");
//			}
//
//			return result;
//		}
//
//		private static bool SetProperty(object target, string propertyName, ProxyType proxyType, object value)
//		{
//			Type targetType = target.GetType();
//			PropertyInfo propertyInfo = targetType.GetProperty(propertyName);
//			if (propertyInfo == null)
//			{
//				return false;
//			}
//
//			if (!propertyInfo.CanWrite)
//			{
//				return false;
//			}
//
//			Type requiredType = propertyInfo.PropertyType;
//
//			if (!requiredType.IsAssignableFrom(value.GetType()))
//			{
//				// Not directly assignable, so we need to create a duck proxy.
//				// This is not possible unless the required type is an interface
//				if (!requiredType.IsInterface)
//				{
//					return false;
//				}
//
//				// create a duck proxy
//				value = ProxyFactory.CreateProxy(requiredType, proxyType, value);
//			}
//
//			propertyInfo.SetValue(target, value, null);
//			return true;
//		}
//
//		private static bool SetMethod(object target, string methodName, ProxyType proxyType, object value)
//		{
//			Type targetType = target.GetType();
//			MethodInfo methodInfo = targetType.GetMethod(methodName);
//			if (methodInfo == null)
//			{
//				return false;
//			}
//
//			ParameterInfo[] parameters = methodInfo.GetParameters();
//			if (parameters.Length != 1)
//			{
//				return false;
//			}
//
//			ParameterInfo parameterInfo = parameters[0];
//			Type requiredType = parameterInfo.ParameterType;
//
//			if (!requiredType.IsAssignableFrom(value.GetType()))
//			{
//				// Not directly assignable, so we need to create a duck proxy.
//				// This is not possible unless the required type is an interface
//				if (!requiredType.IsInterface)
//				{
//					return false;
//				}
//
//				// create a proxy
//				value = ProxyFactory.CreateProxy(requiredType, proxyType, value);
//			}
//
//			methodInfo.Invoke(target, new[] {value});
//			return true;
//		}
//	}
//}