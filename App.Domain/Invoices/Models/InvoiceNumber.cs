using App.Domain.Deliveries.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace App.Domain.Invoices.Models
{
    public record InvoiceNumber
    {

        public const string Pattern = "INV[0-9]{3}$";
        private static readonly Regex PatternRegex = new(Pattern);

        public string Value { get; }
        public InvoiceNumber(string value)
        {
            if (IsValid(value))
            {
                Value = value;
            }
            else
            {
                throw new InvalidInvoiceNumberException("");
            }
        }

        private static bool IsValid(string value) => PatternRegex.IsMatch(value);

        public static Option<InvoiceNumber> TryParse(string valueString)
        {
            if (IsValid(valueString))
            {
                return Some<InvoiceNumber>(new(valueString));
            }
            return None;
        }

    }
}
