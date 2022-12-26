using Exemple.Domain.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LanguageExt.Prelude;
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

        public static Option<DeliveryNumber> TryParse(string valueString)
        {
            if (IsValid(valueString))
            {
                return Some<DeliveryNumber>(new(valueString));
            }
            return None;
        }

    }
}
