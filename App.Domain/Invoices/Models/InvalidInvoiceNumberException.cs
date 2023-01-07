using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Invoices.Models
{
 
    [Serializable]
    internal class InvalidInvoiceNumberException : Exception
    {
        public InvalidInvoiceNumberException()
        {
        }

        public InvalidInvoiceNumberException(string? message) : base(message)
        {
        }

        public InvalidInvoiceNumberException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidInvoiceNumberException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
