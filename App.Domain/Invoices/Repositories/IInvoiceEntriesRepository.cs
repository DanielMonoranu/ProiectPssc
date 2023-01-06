using App.Domain.Deliveries.Models;
using App.Domain.Invoices.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Deliveries.Models.Deliveries;
using static App.Domain.Invoices.Models.Invoices;

namespace App.Domain.Invoices.Repositories
{
    
    public interface IInvoiceEntriesRepository
    {
        TryAsync<List<CancelledInvoice>> TryGetExistingInvoice();
        TryAsync<Unit> TrySaveInvoice(AnnouncedInvoices deliveries);
    }
}
