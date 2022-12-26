using Domain.Models;
using LanguageExt;
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

        public async Task <ICancelDeliveryEvent >ExecuteAsync(CancelDeliveryCommand command, Func<DeliveryNumber, TryAsync<bool>> checkDeliveryExists)
        {
            UnvalidatedDeliveries unvalidatedDeliveries = new UnvalidatedDeliveries(command.InputDeliveries);
            IDeliveries deliveries = await ValidateDeliveries(checkDeliveryExists, unvalidatedDeliveries);
            deliveries = CancelDeliveries(deliveries);

            return deliveries.Match(
                whenUnvalidatedDeliveries: unvalidDeliveries => new CancelDeliveryFailedEvent("Unexpected unvalidated state of delivery") as ICancelDeliveryEvent,
                whenInvalidatedDeliveries: invalidDeliveries => new CancelDeliveryFailedEvent(invalidDeliveries.Reason),
                whenValidatedDeliveries: validDeliveries => new CancelDeliveryFailedEvent("Unexpected validated state"),
                whenCancelledDeliveries: cancelledDeliveries => new CancelDeliverySucceededEvent(cancelledDeliveries.MessageShown, cancelledDeliveries.CancelDate)
                );

        }

    }
}
