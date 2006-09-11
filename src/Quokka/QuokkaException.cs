using System;
using System.Runtime.Serialization;

namespace Quokka
{
    /// <summary>
    /// Base class for all Quokka-specific exceptions
    /// </summary>
    public class QuokkaException : Exception
    {
        public QuokkaException() { }
        public QuokkaException(string message) : base(message) { }
        public QuokkaException(string message, Exception innerException) : base(message, innerException) { }
        protected QuokkaException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
