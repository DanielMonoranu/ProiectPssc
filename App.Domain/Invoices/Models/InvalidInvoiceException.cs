using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Invoices.Models
{
   
    [Serializable]
    internal class InvalidInvoiceException : Exception
    {
        public InvalidInvoiceException()
        {
        }

        public InvalidInvoiceException(string? message) : base(message)
        {
        }

        public InvalidInvoiceException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidInvoiceException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
