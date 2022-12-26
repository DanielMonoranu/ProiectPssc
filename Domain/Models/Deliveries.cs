using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [AsChoice]
    public static partial class Deliveries
    {
        public interface IDeliveries { }

        //validated, unvalidated delivery?



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
            public InvalidatedDeliveries(IReadOnlyCollection<UnvalidatedDelivery> deliveryList, string reason)
            {
                DeliveryList = deliveryList;
                Reason = reason;
            }

            public IReadOnlyCollection<UnvalidatedDelivery> DeliveryList { get; }
            public string Reason { get; }
        }




        public record ValidatedDeliveries : IDeliveries
        {
            public ValidatedDeliveries(IReadOnlyCollection<ValidatedDelivery> deliveriesList)
            {
                DeliveryList = deliveriesList;
            }
            public IReadOnlyCollection<ValidatedDelivery> DeliveryList { get; }

        }


        public record CancelledDeliveries : IDeliveries
        {
            public CancelledDeliveries(IReadOnlyCollection<CancelledDelivery> deliveriesList, string messageShown, DateTime cancelDate)
            {
                DeliveryList = deliveriesList;
                MessageShown = messageShown;
                CancelDate = cancelDate;
            }
            public IReadOnlyCollection<CancelledDelivery> DeliveryList { get; }
            public DateTime CancelDate { get; }
            public string MessageShown { get; }

        }

    }
}
