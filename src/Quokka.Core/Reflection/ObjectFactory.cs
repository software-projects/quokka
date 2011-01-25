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
using Common.Logging;

namespace Quokka.Reflection
{
	[Obsolete("Object factory is not used anymore. Will be deleted soon.")]
	public static class ObjectFactory
	{
		private static readonly ILog _logger = LogManager.GetCurrentClassLogger();

		public static object Create(Type objectType, IServiceProvider serviceProvider, params object[] concreteObjects)
		{
			if (objectType == null)
				throw new ArgumentNullException("objectType");

			ConstructorInfo constructor = ChooseConstructor(objectType);

			ParameterInfo[] parameters = constructor.GetParameters();
			object[] parameterValues = new object[parameters.Length];

			for (int index = 0; index < parameters.Length; ++index)
			{
				parameterValues[index] = GetParameterValue(parameters[index], serviceProvider, concreteObjects);
			}

			try
			{
				return constructor.Invoke(parameterValues);
			}
			catch (Exception ex)
			{
				ArgumentNullException innerException = ex.InnerException as ArgumentNullException;
				if (innerException == null)
				{
					string message = String.Format("Failed to create object of type {0}", objectType.FullName);
					_logger.Error(message, ex);
					throw new QuokkaException(message, ex);
				}
				else
				{
					// Special case where an object has failed to be constructed because
					// of a missing parameter. Attempt to throw a more meaningful exception.
					string message;
					if (String.IsNullOrEmpty(innerException.ParamName))
					{
						message = String.Format("Failed to create object of type {0} because of a missing parameter", objectType.FullName);
					}
					else
					{
						message =
							String.Format("Failed to create object of type {0} because of missing parameter: {1}", objectType.FullName,
							              innerException.ParamName);
					}

					_logger.Error(message, ex);
					throw new QuokkaException(message, ex);
				}
			}
		}

		private static object GetParameterValue(ParameterInfo parameterInfo, IServiceProvider serviceProvider,
		                                        object[] concreteObjects)
		{
			if (concreteObjects != null)
			{
				// Attempt to find an exact match from the concrete objects
				foreach (object concreteObject in concreteObjects)
				{
					if (concreteObject != null)
					{
						if (parameterInfo.ParameterType == concreteObject.GetType())
						{
							return concreteObject;
						}
					}
				}

				// Attempt to find an acceptable match from the concrete objects
				// TODO: makes no attempt to find the best match if there are two, maybe should throw an exception
				foreach (object concreteObject in concreteObjects)
				{
					if (concreteObject != null)
					{
						if (parameterInfo.ParameterType.IsAssignableFrom(concreteObject.GetType()))
						{
							return concreteObject;
						}
					}
				}
			}

			if (serviceProvider != null)
			{
				if (parameterInfo.ParameterType.IsAssignableFrom(serviceProvider.GetType()))
				{
					// constructor parameter is for a service provider or service container
					return serviceProvider;
				}

				if (parameterInfo.ParameterType.IsInterface)
				{
					// constructor parameter is for an interface, which is expected to come from the provider
					return serviceProvider.GetService(parameterInfo.ParameterType);
				}
			}

			// cannot find a suitable parameter
			return null;
		}

		/// <summary>
		/// Choose a constructor to use to create an object of the specified type.
		/// </summary>
		/// <param name="objectType">Type to create</param>
		/// <returns>A public constructor.</returns>
		private static ConstructorInfo ChooseConstructor(Type objectType)
		{
			ConstructorInfo chosenConstructor = null;

			foreach (ConstructorInfo constructor in objectType.GetConstructors())
			{
				if (chosenConstructor == null)
				{
					chosenConstructor = constructor;
				}
				else if (chosenConstructor.GetParameters().Length < constructor.GetParameters().Length)
				{
					// always choose the constructor with the most arguments
					// TODO: undefined which constructor to choose if multiple constructors have the same
					// number of arguments.
					chosenConstructor = constructor;
				}
			}

			if (chosenConstructor == null)
			{
				string message = "No public constructors for type: " + objectType;
				_logger.Error(message);
				throw new QuokkaException(message);
			}

			return chosenConstructor;
		}
	}
}