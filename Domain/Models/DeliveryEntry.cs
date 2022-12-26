using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static bool TryParseStatus(string statusString,string orderString, out DeliveryEntry delivery)
        {
            bool isValid = false;
            delivery = null;
            if(int.TryParse(statusString,out int numericStatus)&&int.TryParse(orderString,out int numericOrder))
            {
                if (IsValid(numericStatus))
                {
                    isValid= true;
                    delivery = new(numericStatus, numericOrder);


                }
            }
            return isValid;
        }


        private static bool IsValid(int status) => status == 0 || status == 1 || status == 2 || status == 3;

    }
}
