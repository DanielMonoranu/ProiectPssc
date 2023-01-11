using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Deliveries.Models
{
    public record UnvalidatedDelivery(string DeliveryNumber, string Status, string orderId);

}
