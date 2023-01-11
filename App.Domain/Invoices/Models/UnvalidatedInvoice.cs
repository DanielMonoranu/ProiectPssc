using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Invoices.Models
{
    public record UnvalidatedInvoice(string InvoiceNumber, string Status, string Vat);

}
