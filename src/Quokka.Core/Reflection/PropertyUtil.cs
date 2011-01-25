#region Copyright notice
//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2006-2011 John Jeffery. All rights reserved.
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
using System.Reflection;

namespace Quokka.Reflection
{
	/// <summary>
	///     Utility functions for accessing object properties.
	/// </summary>
	public static class PropertyUtil
	{
        public static object GetValue(object obj, string propertyName) {
            return GetValue(obj, propertyName, null);
        }

        public static object GetValue(object obj, string propertyName, object defaultValue) {
            if (obj == null) {
                throw new ArgumentNullException("obj");
            }
            if (propertyName == null) {
                throw new ArgumentNullException("propertyName");
            }

            Type type = obj.GetType();
            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null) {
                return defaultValue;
            }

            object propertyValue = propertyInfo.GetValue(obj, null);
            return propertyValue;
        }

        public static string GetStringValue(object obj, string propertyName, string defaultValue) {
            object propertyValue = GetValue(obj, propertyName, defaultValue);
            if (propertyValue == null) {
                return null;
            }
            return propertyValue.ToString();
        }

        public static bool TrySetValue(object obj, string propertyName, object propertyValue) {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");

            Type objType = obj.GetType();

            PropertyInfo propertyInfo = objType.GetProperty(propertyName);
            if (propertyInfo == null || !propertyInfo.CanWrite) {
                return false;
            }

            object convertedValue;
            try {
                convertedValue = Convert.ChangeType(propertyValue, propertyInfo.PropertyType);
            }
            catch (Exception) {
                return false;
            }

            try {
                propertyInfo.SetValue(obj, convertedValue, null);
            }
            catch (Exception) {
                return false;
            }

            return true;
        }

        public static string GetStringValue(object obj, string propertyName) {
            return GetStringValue(obj, propertyName, null);
        }

        public static void CopyProperties(object from, object to) {
            if (from == null) {
                throw new ArgumentNullException("from");
            }
            if (to == null) {
                throw new ArgumentNullException("to");
            }
            Type fromType = from.GetType();

        	foreach (PropertyInfo fromProperty in fromType.GetProperties()) {
                if (fromProperty.CanRead) {
                    object fromValue = fromProperty.GetValue(from, null);
                    TrySetValue(to, fromProperty.Name, fromValue);
                }
            }
        }

        public static void SetValues(object obj, PropertyCollection properties) {
            if (obj == null)
                throw new ArgumentNullException("obj");
            if (properties == null) {
                return;
            }

            foreach (string name in properties.GetAllKeys()) {
                TrySetValue(obj, name, properties[name]);
            }
        }
    }
}
