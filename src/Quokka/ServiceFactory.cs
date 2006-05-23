
namespace Quokka
{
    using System;
    using System.Collections.Generic;
    using SCMD = System.ComponentModel.Design;
    using System.Reflection;

    internal class ServiceFactory
    {
        private Dictionary<Type, Type> serviceTypeDict = new Dictionary<Type, Type>();
        private SCMD.ServiceCreatorCallback serviceCreatorCallback;
        private List<Type> underConstruction = new List<Type>();

        public ServiceFactory() {
            serviceCreatorCallback = new SCMD.ServiceCreatorCallback(CreateServiceInstance);
        }

        public SCMD.ServiceCreatorCallback AddService(Type serviceType, Type serviceInstanceType) {
            if (serviceType == null)
                throw new ArgumentNullException("serviceType");
            if (serviceInstanceType == null)
                throw new ArgumentNullException("serviceInstanceType");

            if (!serviceType.IsAssignableFrom(serviceInstanceType)) {
                throw new ArgumentException("Type is not compatible", "serviceInstanceType");
            }

            ConstructorInfo[] constructors = serviceInstanceType.GetConstructors();

            if (constructors.Length > 1) {
                string message = String.Format("Too many constructors for {0}", serviceInstanceType);
                throw new ArgumentException(message);
            }

            if (constructors.Length == 0) {
                string message = String.Format("No public constructor for {0}", serviceInstanceType);
                throw new ArgumentException(message);
            }

            ConstructorInfo constructor = constructors[0];

            if (constructor.ContainsGenericParameters) {
                string message = String.Format("Unassigned generic parameters in type {0}", serviceInstanceType);
                throw new ArgumentException(message);
            }

            foreach (ParameterInfo parameter in constructor.GetParameters()) {
                if (!parameter.ParameterType.IsInterface) {
                    string message = String.Format("Constructor for {0} has a non-interface parameter: {1}", serviceInstanceType, parameter);
                    throw new ArgumentException(message);
                }
            }

            serviceTypeDict.Add(serviceType, serviceInstanceType);

            return serviceCreatorCallback;
        }

        public void RemoveService(Type serviceType) {
            serviceTypeDict.Remove(serviceType);
        }

        private object CreateServiceInstance(SCMD.IServiceContainer container, Type serviceType) {
            Type serviceInstanceType;

            if (!serviceTypeDict.TryGetValue(serviceType, out serviceInstanceType)) {
                // unknown type
                return null;
            }

            if (underConstruction.Contains(serviceType)) {
                string message = String.Format("Circular reference detected for {0}", serviceType);
                throw new QuokkaException(message);
            }

            underConstruction.Add(serviceType);
            try {

                ConstructorInfo constructor = serviceInstanceType.GetConstructors()[0];
                ParameterInfo[] parameterInfos = constructor.GetParameters();
                object[] parameterValues = new object[parameterInfos.Length];
                for (int index = 0; index < parameterInfos.Length; ++index) {
                    ParameterInfo parameterInfo = parameterInfos[index];
                    object parameterValue = container.GetService(parameterInfo.ParameterType);
                    if (parameterValue == null) {
                        if (parameterInfo.ParameterType == typeof(IServiceProvider)
                            || parameterInfo.ParameterType == typeof(SCMD.IServiceContainer)) {
                            parameterValue = container;
                        }
                        else {
                            string message = String.Format("No available implementation of {0}", parameterInfo.ParameterType);
                            throw new QuokkaException(message);
                        }
                    }
                    parameterValues[index] = parameterValue;
                }

                object serviceInstance = constructor.Invoke(parameterValues);
                return serviceInstance;
            }
            finally {
                underConstruction.Remove(serviceType);
            }
        }
    }
}
