using System;
using System.Collections.Specialized;
using System.Reflection;

namespace Quokka.Reflection
{
    public static class ObjectFactory
    {
        public static object Create(Type objectType, IServiceProvider serviceProvider, params object[] concreteObjects) {
            if (objectType == null)
                throw new ArgumentNullException("objectType");

            ConstructorInfo constructor = ChooseConstructor(objectType);

            ParameterInfo[] parameters = constructor.GetParameters();
            object[] parameterValues = new object[parameters.Length];

            for (int index = 0; index < parameters.Length; ++index) {
                parameterValues[index] = GetParameterValue(parameters[index], serviceProvider, concreteObjects);
            }

            return constructor.Invoke(parameterValues);
        }

        private static object GetParameterValue(ParameterInfo parameterInfo, IServiceProvider serviceProvider, object[] concreteObjects) {
            if (concreteObjects != null) {
                // Attempt to find an exact match from the concrete objects
                foreach (object concreteObject in concreteObjects) {
                    if (concreteObject != null) {
                        if (parameterInfo.ParameterType == concreteObject.GetType()) {
                            return concreteObject;
                        }
                    }
                }

                // Attempt to find an acceptable match from the concrete objects
                // TODO: makes no attempt to find the best match if there are two, maybe should throw an exception
                foreach (object concreteObject in concreteObjects) {
                    if (concreteObject != null) {
                        if (parameterInfo.ParameterType.IsAssignableFrom(concreteObject.GetType())) {
                            return concreteObject;
                        }
                    }
                }
            }

            if (serviceProvider != null) {
                if (parameterInfo.ParameterType.IsAssignableFrom(serviceProvider.GetType())) {
                    // constructor parameter is for a service provider or service container
                    return serviceProvider;
                }

                if (parameterInfo.ParameterType.IsInterface) {
                    // constructor parameter is for an interface, which is expected to come from the provider
                    return serviceProvider.GetService(parameterInfo.ParameterType);
                }
            }

            // cannot find a suitable parameter
            return null;
        }

        /// <summary>
        /// Choose a constructor to use to create an object of the specified type.
        /// </summary>
        /// <param name="objectType">Type to create</param>
        /// <returns>A public constructor.</returns>
        private static ConstructorInfo ChooseConstructor(Type objectType) {
            ConstructorInfo chosenConstructor = null;

            foreach (ConstructorInfo constructor in objectType.GetConstructors()) {
                if (chosenConstructor == null) {
                    chosenConstructor = constructor;
                }
                else if (chosenConstructor.GetParameters().Length < constructor.GetParameters().Length) {
                    // always choose the constructor with the most arguments
                    // TODO: undefined which constructor to choose if multiple constructors have the same
                    // number of arguments.
                    chosenConstructor = constructor;
                }
            }

            if (chosenConstructor == null) {
                throw new QuokkaException("No public constructors for type: " + objectType.ToString());
            }

            return chosenConstructor;
        }
    }
}
