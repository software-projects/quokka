using System;
using System.Reflection;
using Quokka.DynamicCodeGeneration;

namespace Quokka.Uip
{
    public static class UipUtils
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
        public static void SetController(object view, object controller) {
            Type viewType = view.GetType();
            MethodInfo methodInfo = viewType.GetMethod("SetController");
            if (methodInfo == null) {
                throw new QuokkaException("Missing method: SetController");
            }

            ParameterInfo[] parameters = methodInfo.GetParameters();
            if (parameters.Length != 1) {
                throw new QuokkaException("Unexpected number of parameters for SetController method");
            }

            ParameterInfo parameterInfo = parameters[0];
            Type requiredControllerType = parameterInfo.ParameterType;

            if (!requiredControllerType.IsAssignableFrom(controller.GetType())) {
                // Not directly assignable, so we need to create a duck proxy.
                // This is not possible unless the required type is an interface
                if (!requiredControllerType.IsInterface) {
                    throw new QuokkaException("Cannot assign controller to view, and cannot create a proxy");
                }

                // create a duck proxy
                controller = ProxyFactory.CreateDuckProxy(requiredControllerType, controller);
            }

            methodInfo.Invoke(view, new object[] { controller });
        }
    }
}
