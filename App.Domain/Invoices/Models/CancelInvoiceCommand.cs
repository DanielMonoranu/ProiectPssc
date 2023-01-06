using App.Domain.Deliveries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Invoices.Models
{
    public record CancelInvoiceCommand
    {
        public CancelInvoiceCommand(IReadOnlyCollection<UnvalidatedInvoice> inputInvoices)
        {
            InputInvoices = inputInvoices;
        }
        public IReadOnlyCollection<UnvalidatedInvoice> InputInvoices { get; }

    }
}
