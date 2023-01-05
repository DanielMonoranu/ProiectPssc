using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace App.Domain.Deliveries.Models
{
    public record DeliveryNumber
    {
        public const string Pattern = "CO[0-9]{3}$";
        private static readonly Regex PatternRegex = new(Pattern);
        public string Value { get; }
        public DeliveryNumber(string value)
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

        private static bool IsValid(string value) => PatternRegex.IsMatch(value);

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
