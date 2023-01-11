using App.Data.Deliveries.Models;
using App.Domain.Deliveries.Models;
using App.Domain.Deliveries.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static App.Domain.Deliveries.Models.Deliveries;
using static LanguageExt.Prelude;


namespace App.Data.Deliveries.Repositories
{
    public class EntriesRepository : IEntriesRepository
    {
        private readonly DeliveriesContext _deliveriesContext;
        public EntriesRepository(DeliveriesContext deliveriesContext)
        {
            _deliveriesContext = deliveriesContext;
        }

        public TryAsync<List<CancelledDelivery>> TryGetExistingDelivery() => async () => (await (
                from e in _deliveriesContext.Entries
                join d in _deliveriesContext.Deliveries on e.DeliveryId equals d.DeliveryId
                select new { d.DeliveryNumber, e.EntryId, e.Status, e.OrderId })
                .AsNoTracking().ToListAsync())
                .Select(result => new CancelledDelivery(
                    DeliveryNumber: new(result.DeliveryNumber),
                    DeliveryEntry: new(result.Status??0, result.OrderId ?? 0))
                { EntryId = result.EntryId }
                    ).ToList();





        public TryAsync<Unit> TrySaveDelivery(AnnouncedDeliveries deliveryEntries) => async () =>
        {
            var deliveries = (await _deliveriesContext.Deliveries.ToListAsync()).ToLookup(delivery => delivery.DeliveryNumber);
            var newEntries = deliveryEntries.DeliveryList
                                         .Where(e => e.IsUpdated && e.EntryId == 0)
                                         .Select(e => new EntryDto()
                                         {
                                             DeliveryId = deliveries[e.DeliveryNumber.Value].Single().DeliveryId,
                                             Status = 0,
                                             OrderId = e.DeliveryEntry.OrderId
                                         });
            var updatedEntries = deliveryEntries.DeliveryList
                                         .Where(e => e.IsUpdated && e.EntryId > 0)
                                         .Select(e => new EntryDto()
                                         {
                                             EntryId = e.EntryId,
                                             DeliveryId = deliveries[e.DeliveryNumber.Value].Single().DeliveryId,
                                             Status = 0,
                                             OrderId = e.DeliveryEntry.OrderId
                                         });
            _deliveriesContext.AddRange(newEntries);
            foreach (var entity in updatedEntries)
            {
                _deliveriesContext.Entry(entity).State = EntityState.Modified;
            }
            await _deliveriesContext.SaveChangesAsync();
            return unit;
        };
    }
}
