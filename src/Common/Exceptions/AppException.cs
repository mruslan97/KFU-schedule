using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    public class AppException : Exception
    {
        /// <summary> Конструктор </summary>
        public AppException(string message)
            : base(message)
        { }

        /// <summary> Конструктор </summary>
        public AppException()
        { }

        /// <summary> Конструктор </summary>
        protected AppException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }

        /// <summary> Конструктор </summary>
        public AppException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}