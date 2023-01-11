using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Invoices.Models
{
 
    [AsChoice]
    public static partial class CancelInvoiceEvent
    {
        public interface ICancelInvoiceEvent { }
        public record CancelInvoiceSucceededEvent : ICancelInvoiceEvent
        {
            public string MessageShown { get; }
            public DateTime CancelDate { get; }
            internal CancelInvoiceSucceededEvent(string message, DateTime cancelDate)
            {
                MessageShown = message;
                CancelDate = cancelDate;
            }
        }

        public record CancelInvoiceFailedEvent : ICancelInvoiceEvent
        {
            public string Reason { get; }
            internal CancelInvoiceFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
