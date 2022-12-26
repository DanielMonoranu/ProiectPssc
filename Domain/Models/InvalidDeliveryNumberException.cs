using System;
using System.Runtime.Serialization;

namespace Exemple.Domain.Models
{
    [Serializable]
    internal class InvalidDeliveryNumberException : Exception
    {
        public InvalidDeliveryNumberException()
        {
        }

        public InvalidDeliveryNumberException(string? message) : base(message)
        {
        }

        public InvalidDeliveryNumberException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidDeliveryNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}