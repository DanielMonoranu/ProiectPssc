using App.Domain.Deliveries.Models;
using App.Domain.Deliveries.Repositories;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Deliveries.Repositories
{
    public class DeliveriesRepository : IDeliveriesRepository
    {
        private readonly DeliveriesContext _deliveriesContext;

        public DeliveriesRepository(DeliveriesContext deliveriesContext)
        {
            _deliveriesContext = deliveriesContext;
        }

        public TryAsync<List<DeliveryNumber>> TryGetExistingDelivery(IEnumerable<string> deliveryToCheck) => async () =>
        {
            var deliveries = await _deliveriesContext.Deliveries
                                                    .Where(delivery => deliveryToCheck.Contains(delivery.DeliveryNumber))
                                                    .AsNoTracking()
                                                    .ToListAsync();
            return deliveries.Select(delivery => new DeliveryNumber(delivery.DeliveryNumber))
                                                .ToList();

        };
    }
}
