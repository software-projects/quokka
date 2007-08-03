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

namespace Quokka
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.Design;
    using System.Reflection;

    public static class ServiceContainerUtil
    {
        public static void AddService(IServiceContainer container, Type serviceType, Type serviceInstanceType) {
            ServiceCreatorCallback callback = CreateCallback(serviceType, serviceInstanceType);
            container.AddService(serviceType, callback);
        }

        public static void AddService(IServiceContainer container, Type serviceType, Type serviceInstanceType, bool promote) {
            ServiceCreatorCallback callback = CreateCallback(serviceType, serviceInstanceType);
            container.AddService(serviceType, callback, promote);
        }

		public static void AddServices(IServiceContainer container, IList<Type> types)
		{
			if (container == null)
				throw new ArgumentNullException("container");
			if (types == null)
				throw new ArgumentNullException("types");
			if (types.Count % 2 != 0)
				throw new ArgumentException("types must have an even number of items");

			for (int index = 0; index < types.Count; index += 2) {
				Type serviceType = types[index];
				Type serviceInstanceType = types[index + 1];
				AddService(container, serviceType, serviceInstanceType);
			}
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

        #region TypeDictionary

        private interface ITypeDictionary
        {
            bool TryGetValue(Type instanceType, out object obj);
            void Add(object obj);
        }

        private class TypeDictionary : ITypeDictionary
        {
            private Dictionary<Type, WeakReference> dict = new Dictionary<Type, WeakReference>();

            #region ITypeDictionary Members

            public bool TryGetValue(Type instanceType, out object obj) {
                obj = null;
                if (instanceType == null)
                    throw new ArgumentNullException();
                WeakReference weakRef;
                if (!dict.TryGetValue(instanceType, out weakRef)) {
                    return false;
                }

                obj = weakRef.Target;
                if (obj == null) {
                    dict.Remove(instanceType);
                }

                return (obj != null);
            }

            public void Add(object obj) {
                if (obj == null)
                    throw new ArgumentNullException();
                dict.Add(obj.GetType(), new WeakReference(obj));
            }

            #endregion
        }

        #endregion

        #region ServiceCreator

        private class ServiceCreator
        {
            private ConstructorInfo constructor;
            private static object lockObject = new object();

            public ServiceCreator(ConstructorInfo constructor) {
                this.constructor = constructor;
            }

            public object ServiceCreatorCallback(IServiceContainer container, Type serviceType) {
                object serviceInstance;
                ITypeDictionary typeDict;
                ICircularReferenceDetector detector;

                lock (lockObject) {
                    typeDict = (ITypeDictionary)container.GetService(typeof(ITypeDictionary));
                    if (typeDict == null) {
                        typeDict = new TypeDictionary();
                        container.AddService(typeof(ITypeDictionary), typeDict, true);
                    }
                    else {
                        if (typeDict.TryGetValue(constructor.DeclaringType, out serviceInstance)) {
                            return serviceInstance;
                        }
                    }

                    detector = (ICircularReferenceDetector)container.GetService(typeof(ICircularReferenceDetector));
                    if (detector == null) {
                        detector = new CircularReferenceDetector();
                        container.AddService(typeof(ICircularReferenceDetector), detector, true);
                    }
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

                    serviceInstance = constructor.Invoke(parameterValues);
                    typeDict.Add(serviceInstance);
                    return serviceInstance;
                }
                finally {
                     detector.Remove(serviceType);
                }
            }
        }

        #endregion
    }
}
