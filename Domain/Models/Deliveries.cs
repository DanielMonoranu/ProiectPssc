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

        public record ValidDeliveries : IDeliveries
        {
            public ValidDeliveries(IReadOnlyCollection<ValidDelivery> deliveriesList)
            {
                DeliveryList = deliveriesList;
            }
            public IReadOnlyCollection<ValidDelivery> DeliveryList { get; }

        }

        public record ShippedDeliveries : IDeliveries
        {
            public ShippedDeliveries(IReadOnlyCollection<ShippedDelivery> deliveriesList)
            {
                DeliveryList = deliveriesList;
            }
            public IReadOnlyCollection<ShippedDelivery> DeliveryList { get; }
        }

        public record CancelledDeliveries : IDeliveries
        {
            public CancelledDeliveries(IReadOnlyCollection<CancelledDelivery> deliveriesList)
            {
                DeliveryList = deliveriesList;
            }
            public IReadOnlyCollection<CancelledDelivery> DeliveryList { get; }
        }

    }
}
