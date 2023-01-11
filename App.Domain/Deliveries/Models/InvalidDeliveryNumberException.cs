using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Deliveries.Models
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
