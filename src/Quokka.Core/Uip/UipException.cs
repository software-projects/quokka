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
using System.Runtime.Serialization;

namespace Quokka.Uip
{
    /// <summary>
    /// Base class for all exceptions thrown by the UIP framework
    /// </summary>
    public class UipException : QuokkaException
    {
        public UipException() { }
        public UipException(string message) : base(message) { }
        public UipException(string message, Exception innerException) : base(message, innerException) { }
        protected UipException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

    /// <summary>
    /// Thrown when a navigation request is made for an undefined transition.
    /// </summary>
    /// <seealso cref="IUipNavigator"/>
    public class UipUndefinedTransitionException : UipException
    {
        public UipUndefinedTransitionException() : base("Undefined UIP transition") { }
        public UipUndefinedTransitionException(string message) : base(message) { }
        public UipUndefinedTransitionException(string message, Exception innerException) : base(message, innerException) { }
        protected UipUndefinedTransitionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }

	/// <summary>
	/// Thrown when an attempt is made to define a UIP task when a task with the same name already exists.
	/// </summary>
	public class UipTaskAlreadyExistsException : UipException
	{
        public UipTaskAlreadyExistsException() : base("A UIP task with that name already exists") { }
        public UipTaskAlreadyExistsException(string message) : base(message) { }
        public UipTaskAlreadyExistsException(string message, Exception innerException) : base(message, innerException) { }
		protected UipTaskAlreadyExistsException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	public class UipTaskDefinitionCreateException : UipException
	{
        public UipTaskDefinitionCreateException() : base("Failed to create task definition") { }
        public UipTaskDefinitionCreateException(string message) : base(message) { }
        public UipTaskDefinitionCreateException(string message, Exception innerException) : base(message, innerException) { }
		protected UipTaskDefinitionCreateException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	public class UipUnknownTaskException : UipException
	{
        public UipUnknownTaskException() : base("Unknown task") { }
        public UipUnknownTaskException(string message) : base(message) { }
        public UipUnknownTaskException(string message, Exception innerException) : base(message, innerException) { }
		protected UipUnknownTaskException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	public class UipTaskDefinitionNameMismatchException : UipException 
	{
        public UipTaskDefinitionNameMismatchException() : base("Task definition name mismatch") { }
        public UipTaskDefinitionNameMismatchException(string message) : base(message) { }
        public UipTaskDefinitionNameMismatchException(string message, Exception innerException) : base(message, innerException) { }
		protected UipTaskDefinitionNameMismatchException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}

	public class UipDuplicateNodeNameException : UipException
	{
		public UipDuplicateNodeNameException() : base("Duplicate node name") { }
		public UipDuplicateNodeNameException(string message) : base(message) { }
		public UipDuplicateNodeNameException(string message, Exception innerException) : base(message, innerException) { }
		protected UipDuplicateNodeNameException(SerializationInfo info, StreamingContext context) : base(info, context) { }
	}
}
