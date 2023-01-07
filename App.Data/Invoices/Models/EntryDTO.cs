using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Invoices.Models
{
    public class EntryDTO
    {
        public int EntryId { get; set; }
        public int InvoiceID { get; set; }
        public string Status { get; set; }
        public decimal VAT { get; set; }
    }
}
