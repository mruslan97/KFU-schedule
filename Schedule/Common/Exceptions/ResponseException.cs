using System;
using System.Runtime.Serialization;

namespace Common.Exceptions
{
    /// <summary>
    ///     Исключение приложения, где можно отправить код ответа
    /// </summary>
    public class ResponseException : AppException
    {
        /// <summary> Код ответа </summary>
        public int ResponseCode { get; set; }

        /// <summary> Конструктор </summary>
        public ResponseException(string message, int responseCode)
            : base(message)
        {
            ResponseCode = responseCode;
        }

        /// <summary> Конструктор </summary>
        public ResponseException(int requestCode)
        {
            ResponseCode = requestCode;
        }

        /// <summary> Конструктор </summary>
        protected ResponseException(SerializationInfo info, StreamingContext context, int requestCode) : base(info, context)
        {
            ResponseCode = requestCode;
        }

        /// <summary> Конструктор </summary>
        public ResponseException(string message, Exception innerException, int requestCode) : base(message, innerException)
        {
            ResponseCode = requestCode;
        }
    }
}