using LanguageExt;
using Microsoft.CodeAnalysis.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace App.Domain.Models
{
    public record OrderRegistrationNumber
    {
        public const string Pattern = "^RN[0-9]{6}$";
        private static readonly Regex ValidPattern = new(Pattern); // RN si 6 numere de la 0 la 9 

        public string Value { get; }

        public OrderRegistrationNumber(string value) {
            if (IsValid(value))
            {
                Value = value;
            }
            else { 
                throw new InvalidOrderRegistrationNumberException("");
            }
        }

        private static bool IsValid(string stringValue) => ValidPattern.IsMatch(stringValue);


        public override string ToString()
        {
            return Value;
        }

        public static Option<OrderRegistrationNumber> TryParse(string stringValue) {

            if (IsValid(stringValue))
            {
                return Some<OrderRegistrationNumber>(new(stringValue));
            }
            else
            {
                return None;
            }

        }

/*        public static bool TryParse(string stringValue, out OrderRegistrationNumber registrationNumber)
        {
            bool isValid = false;
            registrationNumber = null;

            if (IsValid(stringValue))
            {
                isValid = true;
                registrationNumber = new(stringValue);
            }

            return isValid;
        }*/
    }
}
