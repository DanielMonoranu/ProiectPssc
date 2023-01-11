using App.Domain.Deliveries.Models;
using App.Domain.Invoices.Models;
using System.ComponentModel.DataAnnotations;

namespace App.Api.Models
{
    public class InputInvoice
    {

        [Required]
        [RegularExpression(InvoiceNumber.Pattern)]
        public string InputInvoiceNumber { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string VAT { get; set; }
    }
}
