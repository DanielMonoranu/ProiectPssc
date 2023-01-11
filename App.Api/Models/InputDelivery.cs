using App.Domain.Deliveries.Models;
using System.ComponentModel.DataAnnotations;

namespace App.Api.Models
{
    public class InputDelivery
    {
        [Required]
        [RegularExpression(DeliveryNumber.Pattern)]
        public string InputDeliveryNumber { get; set; }
        [Required]
        public string Status { get; set; }
        [Required]
        public string OrderId { get; set; }
    }
}
