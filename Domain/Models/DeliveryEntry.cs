using Exemple.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
namespace Domain.Models
{
    public record DeliveryEntry
    {
        public int Status { get; }
        public int OrderId { get; }

        public DeliveryEntry(int status, int orderId)
        {
            if (IsValid(status))
            {
                Status = status;
                OrderId = orderId;
            }
            else
            {
                throw new InvalidDeliveryNumberException($"{status} is an invalid status for a delivery.");
            }

        }

        public static Option<DeliveryEntry> TryParseStatus(string statusString, string orderString)
        {
            if (int.TryParse(statusString, out int numericStatus) && int.TryParse(orderString, out int numericOrder) && IsValid(numericStatus))
            {
                return Some<DeliveryEntry>(new(numericStatus, numericOrder));

            }
            else
            {

                return None;
            }
        }


        private static bool IsValid(int status) => status == 0 || status == 1 || status == 2 || status == 3;

    }
}
