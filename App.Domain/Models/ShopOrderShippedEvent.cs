using CSharp.Choices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models
{
    [AsChoice]
    public static partial class ShopOrderShippedEvent
    {
        public interface IShopOrdersShippedEvent { }

        public record ShopOrderShippedScucceededEvent : IShopOrdersShippedEvent
        {
            public string Csv { get; }
            public DateTime ShippedDate { get; }

            internal ShopOrderShippedScucceededEvent(string csv, DateTime publishedDate)
            {
                Csv = csv;
                ShippedDate = publishedDate;
            }
        }

        public record ShopOrderShippedFaildEvent : IShopOrdersShippedEvent
        {
            public string Reason { get; }

            internal ShopOrderShippedFaildEvent(string reason)
            {
                Reason = reason;
            }
        }
    }
}
