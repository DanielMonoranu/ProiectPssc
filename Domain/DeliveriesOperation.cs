using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.Models.Deliveries;

namespace Domain
{
    public static class DeliveriesOperation
    {

        public static IDeliveries CancelDelivery(Func<DeliveryNumber,bool> checkDeliveryExists,ValidDeliveries deliveries)
        {
            List<CancelledDelivery> cancelledDeliveries = new();
            foreach (var delivery in deliveries.DeliveryList)
            {
                /*
                if(!DeliveryNumber.TryParse(delivery.DeliveryNumber,out DeliveryNumber deliveryNumber)
                   && checkDeliveryExists(deliveryNumber))
                {
                }
                */
                CancelledDelivery cancelledDelivery = new(delivery.DeliveryNumber, delivery.Product);
                cancelledDeliveries.Add(cancelledDelivery);
            }
            return new CancelledDeliveries(cancelledDeliveries);
            //sau avem si inca o lista pt nevalidate sau cvvv?
        }
        //alte functii?

    }
}
