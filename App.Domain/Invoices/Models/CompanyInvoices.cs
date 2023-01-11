using App.Domain.Deliveries.Models;
using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Invoices.Models
{
    [AsChoice]
    public static partial class Invoices
    {
        public interface IInvoices { }

        public record UnvalidatedInvoices : IInvoices
        {
            public UnvalidatedInvoices(IReadOnlyCollection<UnvalidatedInvoice> invoiceList)
            {
                InvoiceList = invoiceList;
            }
            public IReadOnlyCollection<UnvalidatedInvoice> InvoiceList { get; }
        }

        public record InvalidatedInvoices : IInvoices
        {
            internal InvalidatedInvoices(IReadOnlyCollection<UnvalidatedInvoice> invoiceList, string reason)
            {
                InvoiceList = invoiceList;
                Reason = reason;
            }

            public IReadOnlyCollection<UnvalidatedInvoice> InvoiceList { get; }
            public string Reason { get; }
        }

        public record FailedInvoices : IInvoices
        {
            internal FailedInvoices(IReadOnlyCollection<UnvalidatedInvoice> invoiceList, Exception exception)
            {
                InvoiceList = invoiceList;
                Exception = exception;
            }
            public IReadOnlyCollection<UnvalidatedInvoice> InvoiceList { get; }
            public Exception Exception { get; }
        }



        public record ValidatedInvoices : IInvoices
        {
            internal ValidatedInvoices(IReadOnlyCollection<ValidatedInvoice> invoiceList)
            {
                InvoiceList = invoiceList;
            }
            public IReadOnlyCollection<ValidatedInvoice> InvoiceList { get; }

        }


        public record CancelledInvoices : IInvoices
        {
            internal CancelledInvoices(IReadOnlyCollection<CancelledInvoice> invoiceList)
            {
                InvoiceList = invoiceList;

            }
            public IReadOnlyCollection<CancelledInvoice> InvoiceList { get; }

        }
        public record AnnouncedInvoices : IInvoices
        {
            internal AnnouncedInvoices(IReadOnlyCollection<CancelledInvoice> invoiceList, string message, DateTime cancelDate)
            {
                InvoiceList = invoiceList;
                Message = message;
                CancelDate = cancelDate;
            }

            public IReadOnlyCollection<CancelledInvoice> InvoiceList { get; }
            public DateTime CancelDate { get; }
            public string Message { get; }
        }

    }
}
