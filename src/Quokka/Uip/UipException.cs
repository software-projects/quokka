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
        public UipUndefinedTransitionException() { }
        public UipUndefinedTransitionException(string message) : base(message) { }
        public UipUndefinedTransitionException(string message, Exception innerException) : base(message, innerException) { }
        protected UipUndefinedTransitionException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
