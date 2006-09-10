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
            Type toType = to.GetType();

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
