using Domain.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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

        public static IDeliveries ValidateDeliveries(Func<DeliveryNumber,bool> checkDeliveryExists, UnvalidatedDeliveries deliveries)
        {
            List<ValidatedDelivery> validatedDeliveries = new();
            bool isValidList = true;
            string invalidReason = string.Empty;
            foreach(var unvalidatedDeliveryEntry in deliveries.DeliveryList)
            {
                if(!DeliveryEntry.TryParseStatus(unvalidatedDeliveryEntry.statusId,unvalidatedDeliveryEntry.orderID,out DeliveryEntry deliveryEntry))
                {
                    invalidReason = $"Invalid status of delivery ({unvalidatedDeliveryEntry.DeliveryNumber},{unvalidatedDeliveryEntry.statusId},{unvalidatedDeliveryEntry.orderID})";
                    isValidList = false;
                    break;
                }
                if(!DeliveryNumber.TryParse(unvalidatedDeliveryEntry.DeliveryNumber,out DeliveryNumber deliveryNumber)
                    && checkDeliveryExists(deliveryNumber)){
                    invalidReason = $"Invalid delivery number ({unvalidatedDeliveryEntry.DeliveryNumber})";
                    isValidList = false;
                    break;
                }
                ValidatedDelivery validatedDelivery = new(deliveryNumber, deliveryEntry);
                validatedDeliveries.Add(validatedDelivery);
            }
            if (isValidList)
            {
                return new ValidatedDeliveries(validatedDeliveries);
            }
            else
            {
                return new InvalidatedDeliveries(deliveries.DeliveryList, invalidReason);
            }

        }


        public static IDeliveries CancelDeliveries(IDeliveries deliveries) =>
            deliveries.Match(
                whenUnvalidatedDeliveries: unvalidatedDelivery => unvalidatedDelivery,
                whenInvalidatedDeliveries: invalidatedDelivery => invalidatedDelivery,
                whenCancelledDeliveries: cancelledDelivery => cancelledDelivery,
                whenValidatedDeliveries: validatedDelivery =>
                {
                    var cancelledDelivery = validatedDelivery.DeliveryList.Select(delivery =>
                        new CancelledDelivery(delivery.DeliveryNumber, delivery.DeliveryEntry));


                    StringBuilder message = new();
                    validatedDelivery.DeliveryList.Aggregate(message, (export, delivery) => export.AppendLine($"{delivery.DeliveryNumber},order status: {delivery.DeliveryEntry.Status}, order Id: {delivery.DeliveryEntry.OrderId}"));
                    CancelledDeliveries cancelledDeliveries = new(cancelledDelivery.ToList().AsReadOnly(), message.ToString(), DateTime.Now);

                    return cancelledDeliveries;
                }
                );
                


         
    }
}


/*
 *  {
                    StringBuilder message = new();
                    validatedDelivery.DeliveryList.Aggregate(message, (export, delivery) => export.AppendLine($"{delivery.DeliveryNumber},order status: {delivery.DeliveryEntry.Status}, order Id: {delivery.DeliveryEntry.OrderId}"));
                    CancelledDeliveries cancelledDeliveries = new(validatedDelivery.DeliveryList, message.ToString(), DateTime.Now);
                    return cancelledDeliveries;
                });*/