using Exemple.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Domain.Models
{
    public record DeliveryNumber
    {
        private static readonly Regex Pattern = new("^CO[0-9]{3}$");
        public string Value { get; }
        private DeliveryNumber(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidDeliveryNumberException("");
            }
        }

        private static bool IsValid(string value) => Pattern.IsMatch(value);

        public static bool TryParse(string valueString,out DeliveryNumber deliveryNumber)
        {
            bool isValid = false;
            deliveryNumber = null;
            if (IsValid(valueString))
            {
                isValid = true;
                deliveryNumber= new DeliveryNumber(valueString);
            }
            return isValid;
        }

    }
}
