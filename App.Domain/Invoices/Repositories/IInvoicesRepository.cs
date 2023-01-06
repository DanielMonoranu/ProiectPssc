using App.Domain.Deliveries.Models;
using App.Domain.Invoices.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Invoices.Repositories
{
     
      public interface IInvoicesRepository
    {
        TryAsync<List<InvoiceNumber>> TryGetExistingInvoice(IEnumerable<string> invoiceToCheck);
    }
}
