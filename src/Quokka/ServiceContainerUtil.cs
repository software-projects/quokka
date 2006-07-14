namespace Quokka
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using SCMD = System.ComponentModel.Design;
    using System.Reflection;

    public static class ServiceContainerUtil
    {
        public static void AddServiceTypeInfo(SCMD.ServiceContainer container, Type serviceType, Type serviceInstanceType) {
            ServiceCreatorCallback callback = CreateCallback(serviceType, serviceInstanceType);
            container.AddService(serviceType, callback);
        }

        public static void AddServiceTypeInfo(SCMD.ServiceContainer container, Type serviceType, Type serviceInstanceType, bool promote) {
            ServiceCreatorCallback callback = CreateCallback(serviceType, serviceInstanceType);
            container.AddService(serviceType, callback, promote);
        }

        public static ServiceCreatorCallback CreateCallback(Type serviceType, Type serviceInstanceType) {
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

            ServiceCreator factory = new ServiceCreator(constructor);
            return new ServiceCreatorCallback(factory.ServiceCreatorCallback);
        }

        #region CircularReferenceDetector

        private interface ICircularReferenceDetector
        {
            bool Contains(Type type);
            void Add(Type type);
            void Remove(Type type);
        }

        private class CircularReferenceDetector : ICircularReferenceDetector
        {
            private List<Type> list = new List<Type>();

            public bool Contains(Type type) {
                return list.Contains(type);
            }

            public void Add(Type type) {
                list.Add(type);
            }

            public void Remove(Type type) {
                list.Remove(type);
            }
        }

        #endregion

        private class ServiceCreator
        {
            private ConstructorInfo constructor;

            public ServiceCreator(ConstructorInfo constructor) {
                this.constructor = constructor;
            }

            public object ServiceCreatorCallback(IServiceContainer container, Type serviceType) {

                ICircularReferenceDetector detector = (ICircularReferenceDetector)container.GetService(typeof(ICircularReferenceDetector));
                if (detector == null) {
                    detector = new CircularReferenceDetector();
                    container.AddService(typeof(ICircularReferenceDetector), detector, true);
                }

                if (detector.Contains(serviceType)) {
                    string message = String.Format("Circular reference detected for {0}", serviceType);
                    throw new QuokkaException(message);
                }

                detector.Add(serviceType);
                try {
                    ParameterInfo[] parameterInfos = constructor.GetParameters();
                    object[] parameterValues = new object[parameterInfos.Length];
                    for (int index = 0; index < parameterInfos.Length; ++index) {
                        ParameterInfo parameterInfo = parameterInfos[index];
                        object parameterValue = container.GetService(parameterInfo.ParameterType);
                        if (parameterValue == null) {
                            if (parameterInfo.ParameterType == typeof(IServiceProvider)
                                || parameterInfo.ParameterType == typeof(IServiceContainer)) {
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
                     detector.Remove(serviceType);
                }
            }
        }
    }
}
