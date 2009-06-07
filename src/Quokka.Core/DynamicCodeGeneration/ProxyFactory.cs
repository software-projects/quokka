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

namespace Quokka.DynamicCodeGeneration
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;

	public enum ProxyType
	{
		DuckProxy,
		NavigatorProxy,
	}

	public static class ProxyFactory
	{
		private static readonly ProxyStore duckProxyStore;
		private static readonly ProxyStore navigatorProxyStore;
		private static DynamicAssembly dynamicAssembly;

		static ProxyFactory()
		{
			duckProxyStore = new ProxyStore();
			navigatorProxyStore = new ProxyStore();
		}

		public static object CreateProxy(Type interfaceType, ProxyType proxyType, object inner)
		{
			switch (proxyType) {
				case ProxyType.DuckProxy:
					return CreateDuckProxy(interfaceType, inner);
				case ProxyType.NavigatorProxy:
					return CreateNavigatorProxy(interfaceType, inner);
				default:
					throw new NotSupportedException();
			}
		}

		#region Duck proxy

		public static T CreateDuckProxy<T>(object inner)
		{
			return (T)CreateDuckProxy(typeof(T), inner);
		}

		public static object CreateDuckProxy(Type interfaceType, object inner)
		{
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
			ConstructorInfo constructor = proxyType.GetConstructor(new Type[] {inner.GetType()});

			// construct an instance of the wrapper class and return
			object proxy = constructor.Invoke(new object[] {inner});
			return proxy;
		}

		public static Type GetDuckProxyType(Type interfaceType, Type innerType)
		{
			Type proxyType = duckProxyStore.Find(interfaceType, innerType);
			if (proxyType == null) {
				proxyType = CreateDuckProxyType(interfaceType, innerType);
				duckProxyStore.Add(interfaceType, innerType, proxyType);
			}
			return proxyType;
		}

		private static Type CreateDuckProxyType(Type interfaceType, Type innerType)
		{
			if (dynamicAssembly == null) {
				dynamicAssembly = new DynamicAssembly();
			}

			return dynamicAssembly.CreateDuckProxyType(interfaceType, innerType);
		}

		#endregion

		#region NavigatorProxy

		public static T CreateNavigatorProxy<T>(object inner)
		{
			return (T)CreateNavigatorProxy(typeof(T), inner);
		}

		public static object CreateNavigatorProxy(Type interfaceType, object inner)
		{
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
			Type proxyType = GetNavigatorProxyType(interfaceType, inner.GetType());

			// get the constructor that accepts inner as a parameter
			ConstructorInfo constructor = proxyType.GetConstructor(new Type[] {inner.GetType()});

			// construct an instance of the wrapper class and return
			object proxy = constructor.Invoke(new object[] {inner});
			return proxy;
		}

		public static Type GetNavigatorProxyType(Type interfaceType, Type innerType)
		{
			Type proxyType = navigatorProxyStore.Find(interfaceType, innerType);
			if (proxyType == null) {
				proxyType = CreateNavigatorProxyType(interfaceType, innerType);
				navigatorProxyStore.Add(interfaceType, innerType, proxyType);
			}
			return proxyType;
		}

		private static Type CreateNavigatorProxyType(Type interfaceType, Type innerType)
		{
			if (dynamicAssembly == null) {
				dynamicAssembly = new DynamicAssembly();
			}

			return dynamicAssembly.CreateNavigatorProxyType(interfaceType, innerType);
		}

		#endregion
	}

	internal class ProxyStore
	{
		private readonly Dictionary<TypePair, Type> m_dict = new Dictionary<TypePair, Type>();

		public Type Find(Type interfaceType, Type innerType)
		{
			TypePair key = new TypePair(interfaceType, innerType);
			Type proxyType;
			m_dict.TryGetValue(key, out proxyType);
			return proxyType;
		}

		public void Add(Type interfaceType, Type innerType, Type proxyType)
		{
			m_dict.Add(new TypePair(interfaceType, innerType), proxyType);
		}

		#region class TypePair

		/// <summary>
		///		Key used in dictionary
		/// </summary>
		private class TypePair
		{
			public readonly Type InterfaceType;
			public readonly Type InnerType;

			public TypePair(Type interfaceType, Type innerType)
			{
				InterfaceType = interfaceType;
				InnerType = innerType;
			}

			public override bool Equals(object obj)
			{
				TypePair other = (TypePair)obj;
				return InterfaceType == other.InterfaceType
				       && InnerType == other.InnerType;
			}

			public override int GetHashCode()
			{
				return InterfaceType.GetHashCode() ^ InnerType.GetHashCode();
			}
		}

		#endregion
	}
}