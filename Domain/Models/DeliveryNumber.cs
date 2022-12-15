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
        public DeliveryNumber(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidStudentRegistrationNumberException("");
             //   throw new Exception("Invalid Pattern of delivery!");
            }
        }

        private static bool IsValid(string value) => Pattern.IsMatch(value);

        public static bool TryParse(string value,out DeliveryNumber deliveryNumber)
        {
            bool isValid = false;
            deliveryNumber = null;
            if (IsValid(value))
            {
                isValid = true;
                deliveryNumber= new DeliveryNumber(value);
            }
            return isValid;
        }

    }
}
