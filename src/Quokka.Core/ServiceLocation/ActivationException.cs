﻿#region License

// Copyright 2004-2014 John Jeffery
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#endregion

using System;
using System.Runtime.Serialization;

namespace Quokka.ServiceLocation
{
	/// <summary>
	/// Exception thrown when a ServiceLocator has an error in resolving an object.
	/// </summary>
	[Serializable]
	public class ActivationException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception"/> class.
		/// </summary>
		public ActivationException()
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public ActivationException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception"/> class with a specified error message and a reference to the inner exception that is the cause of this exception.
		/// 
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.
		/// </param>
		/// <param name="innerException">
		/// The exception that is the cause of the current exception, or a null reference if no inner exception is specified.
		///</param>
		public ActivationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:System.Exception"/> class with serialized data.
		/// </summary>
		/// <param name="info">
		/// The <see cref="T:System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object 
		/// data about the exception being thrown.
		/// </param>
		/// <param name="context">
		/// The <see cref="T:System.Runtime.Serialization.StreamingContext"/> that contains contextual information 
		/// about the source or destination.
		/// </param>
		/// <exception cref="T:System.ArgumentNullException">The <paramref name="info"/> parameter is null.
		/// </exception>
		/// <exception cref="T:System.Runtime.Serialization.SerializationException">
		/// The class name is null or <see cref="P:System.Exception.HResult"/> is zero (0).
		/// </exception>
		protected ActivationException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}