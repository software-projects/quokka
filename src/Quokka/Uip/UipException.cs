using System;
using System.Runtime.Serialization;

namespace Quokka.Uip
{
    public class UipException : QuokkaException
    {
        public UipException() { }
        public UipException(string message) : base(message) { }
        public UipException(string message, Exception innerException) : base(message, innerException) { }
        public UipException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}
