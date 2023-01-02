using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Data.Models
{
    public class OrderDTO
    {
        public int OrderId { get; set; }
        public string OrderRegistrationId { get; set; }
        public decimal? Price { get; set; }
        public decimal? FinalPrice { get; set; }

        public bool IsCancelled { get; set; }
    }
}
