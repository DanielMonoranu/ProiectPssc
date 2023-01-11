using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using App.Domain.Models;

namespace App.Api.Models
{
    public class InputOrder
    {
        [Required]
        [RegularExpression(OrderRegistrationNumber.Pattern)]
        public string OrderRegistrationNumberInput { get; set; }

        [Required]
        [Range(0, 10000)]
        public decimal OrderValue { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int OrderId { get; set; }

        [Required]
        public bool IsCancelled { get; set; }
    }
}
