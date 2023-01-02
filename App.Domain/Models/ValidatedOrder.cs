using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models
{
    public record ValidatedOrder(OrderRegistrationNumber OrderRegistrationNumber, Order Order) { 
        public int OrderId { get; set; }
    }
}
