#region Copyright notice

//
// Authors: 
//  John Jeffery <john@jeffery.id.au>
//
// Copyright (C) 2008 John Jeffery. All rights reserved.
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
using PostSharp.Extensibility;
using PostSharp.Laos;
using Quokka.PresentationModel.Internal;

namespace Quokka.PresentationModel
{
	[Serializable]
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
	public class NotifyChangedAttribute : OnMethodInvocationAspect
	{
		[NonSerialized] private MethodInfo getMethod;
		[NonSerialized] private bool getMethodFound;
		private string propertyName;

		public override bool CompileTimeValidate(MethodBase method)
		{
			if (!base.CompileTimeValidate(method))
				return false;

			if (!method.Name.StartsWith("set_"))
				return false;

			Type actualType = method.DeclaringType;
			Type expectedType = typeof (PresentationObject);
			if (!expectedType.IsAssignableFrom(actualType))
			{
				// TODO: need to find out how to get file and line number
				PresentationMessageSource.Instance.Write(SeverityType.Error, "PRES0001", new object[] {actualType});
				return false;
			}

			PropertyInfo property = GetPropertyForSetMethod(method);
			if (property == null)
			{
				PresentationMessageSource.Instance.Write(SeverityType.Error, "PRES0003", new object[] {method, method.DeclaringType});
				return false;
			}

			if (!property.CanRead)
			{
				PresentationMessageSource.Instance.Write(SeverityType.Error, "PRES0002",
				                                         new object[] {property.Name, method.DeclaringType});
				return false;
			}

			propertyName = property.Name;
			return true;
		}

		public override void OnInvocation(MethodInvocationEventArgs eventArgs)
		{
			if (!getMethodFound)
			{
				FindGetMethod(eventArgs);
			}

			// Because of compile-time checks the getMethod should always be non-null,
			// but there is a check here anyway.
			if (getMethod != null)
			{
				object oldValue = getMethod.Invoke(eventArgs.Delegate.Target, null);
				eventArgs.Proceed();
				object newValue = getMethod.Invoke(eventArgs.Delegate.Target, null);
				PresentationObject.RaisePropertyChangedEventIfNecessary(eventArgs.Delegate.Target, oldValue, newValue, propertyName);
			}
		}

		private void FindGetMethod(MethodInvocationEventArgs e)
		{
			//propertyName = e.Delegate.Method.Name.Substring(5);
			PropertyInfo property = e.Delegate.Target.GetType().GetProperty(propertyName);

			// There should always be a non-null property because of the compile-time check, but
			// check anyway.
			if (property == null)
				return;

			getMethod = property.GetGetMethod();
			getMethodFound = true;
		}

		private static PropertyInfo GetPropertyForSetMethod(MethodBase setMethod)
		{
			foreach (PropertyInfo property in setMethod.DeclaringType.GetProperties())
			{
				if (!property.CanWrite)
					continue;

				if (property.GetSetMethod(true).Name == setMethod.Name)
					return property;
			}

			return null;
		}
	}
}