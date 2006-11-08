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
