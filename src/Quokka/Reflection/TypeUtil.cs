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
