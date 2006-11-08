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

namespace Quokka.Reflection
{
    public static class TypeUtil
    {
        public static Type FindType(string name, IEnumerable<string> namespaces, IEnumerable<Assembly> assemblies) {
            return FindType(name, namespaces, assemblies, false);
        }

        public static Type FindType(string name, IEnumerable<string> namespaces, IEnumerable<Assembly> assemblies, bool throwOnError) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (assemblies == null)
                throw new ArgumentNullException("assemblies");

            foreach (Assembly assembly in assemblies) {
                Type type = FindType(name, namespaces, assembly, false);
                if (type != null) {
                    return type;
                }
            }

            if (throwOnError) {
                throw new QuokkaException("Cannot find type: " + name);
            }

            return null;
        }

        public static Type FindType(string name, IEnumerable<string> namespaces, Assembly assembly) {
            return FindType(name, namespaces, assembly, false);
        }

        public static Type FindType(string name, IEnumerable<string> namespaces, Assembly assembly, bool throwOnError) {
            if (name == null)
                throw new ArgumentNullException("name");
            if (assembly == null)
                throw new ArgumentNullException("assembly");

            // first try without a namespace
            Type type = assembly.GetType(name, false, true);
            if (type != null) {
                return type;
            }

            if (namespaces != null) {
                foreach (string ns in namespaces) {
                    string typeName;

                    if (ns.EndsWith(".")) {
                        typeName = ns + name;
                    }
                    else {
                        typeName = ns + "." + name;
                    }

                    type = assembly.GetType(typeName, false, true);
                    if (type != null) {
                        return type;
                    }
                }
            }

            if (throwOnError) {
                throw new QuokkaException("Cannot find type: " + name);
            }

            // not found
            return null;
        }
    }
}
