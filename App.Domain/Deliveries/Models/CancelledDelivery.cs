using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Deliveries.Models
{
    public record CancelledDelivery(DeliveryNumber DeliveryNumber, DeliveryEntry DeliveryEntry)
    {
        public int EntryId { get; set; }
        public bool IsUpdated { get; set; }
    }
}
