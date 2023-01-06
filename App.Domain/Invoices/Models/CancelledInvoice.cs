using App.Domain.Deliveries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Invoices.Models
{
    public record CancelledInvoice(InvoiceNumber InvoiceNumber, Invoice InvoiceEntry)
    {
        public int EntryId { get; set; }
        public bool IsUpdated { get; set; }
    }
}
