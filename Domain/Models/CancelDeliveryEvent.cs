using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    [AsChoice]
    public static partial class CancelDeliveryEvent
    {
        public interface ICancelDeliveryEvent { }
        public record CancelDeliverySucceededEvent : ICancelDeliveryEvent
        {
            public string csv { get; }
            public DateTime CancelDate { get; }
            internal CancelDeliverySucceededEvent(string csv, DateTime cancelDate)
            {
                this.csv = csv;
                CancelDate = cancelDate;
            }
        }

        public record CancelDeliveryFailedEvent : ICancelDeliveryEvent
        {
            public string Reason { get; }
            internal CancelDeliveryFailedEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
