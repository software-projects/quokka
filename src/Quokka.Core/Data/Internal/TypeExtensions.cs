using System;
using System.Reflection;

namespace Quokka.Data.Internal
{
	internal static class TypeExtensions
	{
		public static PropertyInfo GetPropertyCaseInsensitive(this Type type, string propertyName)
		{
			var properties = type.GetProperties();
			propertyName = (propertyName ?? string.Empty).Trim();

			// attempt a case-sensitive match
			foreach (var property in properties)
			{
				if (property.Name == propertyName)
				{
					return property;
				}
			}

			// second attempt at case-insensitive match
			foreach (var property in properties)
			{
				if (StringComparer.OrdinalIgnoreCase.Compare(property.Name, propertyName) == 0)
				{
					return property;
				}
			}

			return null;
		}
	}

	internal static class PropertyInfoExtensions
	{
		public static bool HasPublicSetter(this PropertyInfo propertyInfo)
		{
			if (!propertyInfo.CanWrite)
			{
				return false;
			}

			var methodInfo = propertyInfo.GetSetMethod();
			if (methodInfo == null || !methodInfo.IsPublic)
			{
				return false;
			}

			return true;
		}
	}
}
