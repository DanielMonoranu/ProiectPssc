using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App.Domain.Models
{
    public record ShipOrderCommand
    {
        public ShipOrderCommand(IReadOnlyCollection<UnvalidatedOrder> inputExamGrades)
        {
            InputExamGrades = inputExamGrades;
        }

        public IReadOnlyCollection<UnvalidatedOrder> InputExamGrades { get; }
    }
}
