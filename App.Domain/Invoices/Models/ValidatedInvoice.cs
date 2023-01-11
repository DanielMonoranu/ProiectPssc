using App.Domain.Deliveries.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Invoices.Models
{
    public record ValidatedInvoice(InvoiceNumber InvoiceNumber, Invoice InvoiceEntry);

}
