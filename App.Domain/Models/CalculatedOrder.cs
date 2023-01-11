using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models
{
    public record CalculatedOrder(OrderRegistrationNumber StudentRegistrationNumber, Order Order, Order FinalOrder) { 
        public int OrderId { get; set; }
    }
}
