using App.Data.Invoices.Models;
using App.Domain.Invoices.Models;
using App.Domain.Invoices.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Invoices.Models.Invoices;
using static LanguageExt.Prelude;

namespace App.Data.Invoices.Repositories
{

    public class InvoiceEntriesRepository : IInvoiceEntriesRepository
    {
        private readonly InvoicesContext _invoicesContext;
        public InvoiceEntriesRepository(InvoicesContext invoicesContext)
        {
            _invoicesContext = invoicesContext;
        }

        public TryAsync<List<CancelledInvoice>> TryGetExistingInvoice() => async () => (await (
                from e in _invoicesContext.Entries
                join d in _invoicesContext.Invoices on e.InvoiceID equals d.InvoiceID
                select new { d.InvoiceNumber, e.EntryId, e.Status, e.VAT })
                .AsNoTracking().ToListAsync())
                .Select(result => new CancelledInvoice(
                    InvoiceNumber: new(result.InvoiceNumber),
                    InvoiceEntry: new(result.Status, result.VAT))
                { EntryId = result.EntryId }).ToList();

        public TryAsync<Unit> TrySaveInvoice(AnnouncedInvoices invoiceEntries) => async () =>
        {
            var invoices = (await _invoicesContext.Invoices.ToListAsync()).ToLookup(invoice => invoice.InvoiceNumber);
            var newEntries = invoiceEntries.InvoiceList
                                         .Where(e => e.IsUpdated && e.EntryId == 0)
                                         .Select(e => new EntryDTO()
                                         {
                                             InvoiceID = invoices[e.InvoiceNumber.Value].Single().InvoiceID,
                                             Status = "Cancelled",
                                             VAT = e.InvoiceEntry.VAT
                                         });
            var updatedEntries = invoiceEntries.InvoiceList
                                         .Where(e => e.IsUpdated && e.EntryId > 0)
                                         .Select(e => new EntryDTO()
                                         {
                                             EntryId = e.EntryId,
                                             InvoiceID = invoices[e.InvoiceNumber.Value].Single().InvoiceID,
                                             Status = "Cancelled",
                                             VAT = e.InvoiceEntry.VAT
                                         });
            _invoicesContext.AddRange(newEntries);
            foreach (var entity in updatedEntries)
            {
                _invoicesContext.Entry(entity).State = EntityState.Modified;
            }
            await _invoicesContext.SaveChangesAsync();
            return unit;
        };


    }
}
