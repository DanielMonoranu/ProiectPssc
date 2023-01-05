using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Deliveries.Models
{
    [AsChoice]
    public static partial class Deliveries
    {
        public interface IDeliveries { }

        public record UnvalidatedDeliveries : IDeliveries
        {
            public UnvalidatedDeliveries(IReadOnlyCollection<UnvalidatedDelivery> deliveryList)
            {
                DeliveryList = deliveryList;
            }
            public IReadOnlyCollection<UnvalidatedDelivery> DeliveryList { get; }
        }

        public record InvalidatedDeliveries : IDeliveries
        {
            internal InvalidatedDeliveries(IReadOnlyCollection<UnvalidatedDelivery> deliveryList, string reason)
            {
                DeliveryList = deliveryList;
                Reason = reason;
            }

            public IReadOnlyCollection<UnvalidatedDelivery> DeliveryList { get; }
            public string Reason { get; }
        }

        public record FailedDeliveries : IDeliveries
        {
            internal FailedDeliveries(IReadOnlyCollection<UnvalidatedDelivery> deliveryList, Exception exception)
            {
                DeliveryList = deliveryList;
                Exception = exception;
            }
            public IReadOnlyCollection<UnvalidatedDelivery> DeliveryList { get; }
            public Exception Exception { get; }
        }



        public record ValidatedDeliveries : IDeliveries
        {
            internal ValidatedDeliveries(IReadOnlyCollection<ValidatedDelivery> deliveriesList)
            {
                DeliveryList = deliveriesList;
            }
            public IReadOnlyCollection<ValidatedDelivery> DeliveryList { get; }

        }


        public record CancelledDeliveries : IDeliveries
        {
            internal CancelledDeliveries(IReadOnlyCollection<CancelledDelivery> deliveriesList)
            {
                DeliveryList = deliveriesList;

            }
            public IReadOnlyCollection<CancelledDelivery> DeliveryList { get; }

        }
        public record AnnouncedDeliveries : IDeliveries
        {
            internal AnnouncedDeliveries(IReadOnlyCollection<CancelledDelivery> deliveriesList, string message, DateTime cancelDate)
            {
                DeliveryList = deliveriesList;
                Message = message;
                CancelDate = cancelDate;
            }

            public IReadOnlyCollection<CancelledDelivery> DeliveryList { get; }
            public DateTime CancelDate { get; }
            public string Message { get; }
        }

    }
}
