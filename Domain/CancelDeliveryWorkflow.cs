using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Domain.DeliveriesOperation;
using static Domain.Models.CancelDeliveryEvent;
using static Domain.Models.Deliveries;

namespace Domain
{
    public class CancelDeliveryWorkflow
    {

        public ICancelDeliveryEvent Execute(CancelDeliveryCommand command, Func<DeliveryNumber, bool> checkDeliveryExists)
        {
            ValidDeliveries validDeliveries = new ValidDeliveries(command.InputDeliveries);
            IDeliveries deliveries = CancelDelivery(checkDeliveryExists, validDeliveries);
            return deliveries.Match(
                whenValidDeliveries: validDeliveries => new CancelDeliveryFailedEvent("Could not cancel delivery") as ICancelDeliveryEvent,
                whenCancelledDeliveries: cancelledDeliveries => new CancelDeliverySucceededEvent("Canceled successfully", DateTime.Now) as ICancelDeliveryEvent,
                whenShippedDeliveries: shippedDeliveries => new CancelDeliveryFailedEvent("Deliveries shipped") as ICancelDeliveryEvent);
        }

    }
}
