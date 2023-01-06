using App.Domain.Deliveries.Models;
using LanguageExt;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LanguageExt.Prelude;

namespace App.Domain.Invoices.Models
{
   public record Invoice
    {

        public string InvoiceStatus { get; }
        public decimal VAT { get; }
        public Invoice(string invoiceStatus,decimal vat)
        {
            if (IsValid(vat))
            {
                InvoiceStatus = invoiceStatus;
                VAT = vat;
            }
            else
            {
                throw new InvalidInvoiceException($"{vat} is an invalid value for the VAT.");

            }
        }

        public static Option<Invoice> TryParseInvoice(string status, string vat)
        {
            if  (decimal.TryParse(vat, out decimal VatNumerical) && IsValid(VatNumerical))
            {
                return Some<Invoice>(new(status, VatNumerical));

            }
            else
            {

                return None;
            }
        }

        private static bool IsValid(decimal numericGrade) => numericGrade > 0 && numericGrade <= 1000;
       
    }
}
