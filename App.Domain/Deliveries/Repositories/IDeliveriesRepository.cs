using App.Domain.Deliveries.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Deliveries.Repositories
{
    public interface IDeliveriesRepository
    {
        TryAsync<List<DeliveryNumber>> TryGetExistingDelivery(IEnumerable<string> deliveryToCheck);
    }
}
