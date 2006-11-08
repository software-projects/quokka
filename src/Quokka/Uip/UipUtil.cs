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
using System.Reflection;
using Quokka.DynamicCodeGeneration;

namespace Quokka.Uip
{
    public static class UipUtil
    {
        /// <summary>
        /// Assign a controller to a view.
        /// </summary>
        /// <param name="view">The view object</param>
        /// <param name="controller">The controller to assign</param>
        /// <remarks>
        /// <para>
        /// This method assigns a controller to a view. It looks for a public
        /// method called <c>SetController</c> that takes one parameter, which
        /// is the controller.
        /// </para>
        /// <para>
        /// If the <c>SetController</c> method requires an interface, and the
        /// controller does not directly implement that interface, then this 
        /// method will attempt to create a 'Duck Proxy'.
        /// </para>
        /// </remarks>
        public static bool SetController(object view, object controller, bool throwOnError) {
            Type viewType = view.GetType();
            MethodInfo methodInfo = viewType.GetMethod("SetController");
            if (methodInfo == null) {
                if (throwOnError) {
                    throw new QuokkaException("Missing method: SetController");
                }
                return false;
            }

            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length != 1) {
                if (throwOnError) {
                    throw new QuokkaException("Unexpected number of parameters for SetController method");
                }
                return false;
            }

            ParameterInfo parameterInfo = parameters[0];
            Type requiredControllerType = parameterInfo.ParameterType;

            if (!requiredControllerType.IsAssignableFrom(controller.GetType())) {
                // Not directly assignable, so we need to create a duck proxy.
                // This is not possible unless the required type is an interface
                if (!requiredControllerType.IsInterface) {
                    if (throwOnError) {
                        throw new QuokkaException("Cannot assign controller to view, and cannot create a proxy");
                    }
                    return false;
                }

                // create a duck proxy
                controller = ProxyFactory.CreateDuckProxy(requiredControllerType, controller);
            }

            methodInfo.Invoke(view, new object[] { controller });
            return true;
        }

        /// <summary>
        /// Assign a state to a view or a controller.
        /// </summary>
        /// <param name="obj">The view or controller object</param>
        /// <param name="state">The state object to assign</param>
        /// <remarks>
        /// <para>
        /// This method assigns a state object to a view or controller. It looks for a public
        /// method called <c>SetState</c> that takes one parameter, which
        /// is the state.
        /// </para>
        /// <para>
        /// If the <c>SetState</c> method requires an interface, and the
        /// controller does not directly implement that interface, then this 
        /// method will attempt to create a 'Duck Proxy'.
        /// </para>
        /// </remarks>
        public static bool SetState(object obj, object state, bool throwOnError) {
            Type viewType = obj.GetType();
            MethodInfo methodInfo = viewType.GetMethod("SetState");
            if (methodInfo == null) {
                if (throwOnError) {
                    throw new QuokkaException("Missing method: SetState");
                }
                return false;
            }

            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length != 1) {
                if (throwOnError) {
                    throw new QuokkaException("Unexpected number of parameters for SetController method");
                }
                return false;
            }

            ParameterInfo parameterInfo = parameters[0];
            Type requiredControllerType = parameterInfo.ParameterType;

            if (!requiredControllerType.IsAssignableFrom(state.GetType())) {
                // Not directly assignable, so we need to create a duck proxy.
                // This is not possible unless the required type is an interface
                if (!requiredControllerType.IsInterface) {
                    if (throwOnError) {
                        throw new QuokkaException("Cannot assign controller to view, and cannot create a proxy");
                    }
                    return false;
                }

                // create a duck proxy
                state = ProxyFactory.CreateDuckProxy(requiredControllerType, state);
            }

            methodInfo.Invoke(obj, new object[] { state });
            return true;
        }
    }
}
