using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models
{
    public record UnvalidatedOrder(string OrderRegistrationNumber, string Grade) { 
        public int OrderId { get; set; }
    }
}
