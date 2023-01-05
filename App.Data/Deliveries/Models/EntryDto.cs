using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Deliveries.Models
{
    public class EntryDto
    {
        public int EntryId { get; set; }
        public int DeliveryId { get; set; }
        public int? Status { get; set; }
        public int? OrderId { get; set; }
    }
}
