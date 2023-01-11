using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppDto
{
    internal record OrderCancelledEvent
    {
        public List<OrderDTO> Orders { get; init; }
    }
}
