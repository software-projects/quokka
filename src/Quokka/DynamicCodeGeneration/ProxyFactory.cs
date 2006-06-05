using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Quokka.DynamicCodeGeneration
{
    public static class ProxyFactory
    {
        private static DuckProxyStore duckProxyStore;
        private static DynamicAssembly dynamicAssembly;

        static ProxyFactory() {
            duckProxyStore = new DuckProxyStore();
        }

        public static T CreateDuckProxy<T>(object inner) {
            return (T)CreateDuckProxy(typeof(T), inner);
        }

        public static object CreateDuckProxy(Type interfaceType, object inner) {
            if (!interfaceType.IsInterface) {
                throw new ArgumentException("Must be an interface", "interfaceType");
            }

            if (inner == null) {
                return null;
            }

            if (interfaceType.IsInstanceOfType(inner)) {
                // when the inner object already supports the interface, do
                // not bother with a proxy
                return inner;
            }

            // Find the proxy type
            Type proxyType = GetDuckProxyType(interfaceType, inner.GetType());
            if (proxyType == null) {
                proxyType = CreateDuckProxyType(interfaceType, inner.GetType());
                duckProxyStore.Add(interfaceType, inner.GetType(), proxyType);
            }

            // get the constructor that accepts inner as a parameter
            ConstructorInfo constructor = proxyType.GetConstructor(new Type[] { inner.GetType() });

            // construct an instance of the wrapper class and return
            object proxy = constructor.Invoke(new object[] { inner });
            return proxy;
        }

        public static Type GetDuckProxyType(Type interfaceType, Type innerType) {
            Type proxyType = duckProxyStore.Find(interfaceType, innerType);
            if (proxyType == null) {
                proxyType = CreateDuckProxyType(interfaceType, innerType);
                duckProxyStore.Add(interfaceType, innerType, proxyType);
            }
            return proxyType;
        }

        private static Type CreateDuckProxyType(Type interfaceType, Type innerType) {
            if (dynamicAssembly == null) {
                dynamicAssembly = new DynamicAssembly();
            }

            return dynamicAssembly.CreateDuckProxyType(interfaceType, innerType);
        }
    }

    internal class DuckProxyStore
    {
        private Dictionary<DictionaryKey, Type> m_dict = new Dictionary<DictionaryKey,Type>();

        public Type Find(Type interfaceType, Type innerType) {
            DictionaryKey key = new DictionaryKey(interfaceType, innerType);
            Type proxyType = null;
            m_dict.TryGetValue(key, out proxyType);
            return proxyType;
        }

        public void Add(Type interfaceType, Type innerType, Type proxyType) {
            m_dict.Add(new DictionaryKey(interfaceType, innerType), proxyType);
        }

        #region class DictionaryKey

        /// <summary>
        ///		Key used in dictionary
        /// </summary>
        private class DictionaryKey
        {
            public readonly Type InterfaceType;
            public readonly Type InnerType;

            public DictionaryKey(Type interfaceType, Type innerType) {
                this.InterfaceType = interfaceType;
                this.InnerType = innerType;
            }

            public override bool Equals(object obj) {
                DictionaryKey other = (DictionaryKey)obj;
                return this.InterfaceType == other.InterfaceType
                    && this.InnerType == other.InnerType;
            }

            public override int GetHashCode() {
                return this.InterfaceType.GetHashCode() ^ this.InnerType.GetHashCode();
            }
        }

        #endregion
    }


}
