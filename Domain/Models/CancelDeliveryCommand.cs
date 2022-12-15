using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record CancelDeliveryCommand
    {
        public CancelDeliveryCommand(IReadOnlyCollection<ValidDelivery> inputDeliveries)
        {
            InputDeliveries = inputDeliveries;
        }
        public IReadOnlyCollection<ValidDelivery> InputDeliveries { get; }

    }
}
