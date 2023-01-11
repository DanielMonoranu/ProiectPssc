using App.Domain.Invoices.Models;
using App.Domain.Invoices.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Invoices.Repositories
{
    
    public class InvoicesRepository : IInvoicesRepository
    {
        private readonly InvoicesContext _invoicesContext;

        public InvoicesRepository(InvoicesContext invoicesContext)
        {
            _invoicesContext = invoicesContext;
        }

        public TryAsync<List<InvoiceNumber>> TryGetExistingInvoice(IEnumerable<string> invoiceToCheck) => async () =>
        {
            var invoices = await _invoicesContext.Invoices
                                                    .Where(invoice => invoiceToCheck.Contains(invoice.InvoiceNumber))
                                                    .AsNoTracking()
                                                    .ToListAsync();
            return invoices.Select(invoice => new InvoiceNumber(invoice.InvoiceNumber))
                                                .ToList();

        };

     
    }
}
