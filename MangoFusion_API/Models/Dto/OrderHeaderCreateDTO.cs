using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MangoFusion_API.Models.Dto
{
    public class OrderHeaderCreateDTO
    {
        [Required]
        public string PickupName { get; set; } = string.Empty;
        [Required]
        public string PickupPhoneNumber { get; set; } = string.Empty;
        [Required]
        public string PickupEmail { get; set; } = string.Empty;
        public string ApplicationUserId { get; set; } = string.Empty;
        public double OrderTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalItem { get; set; }

        public List<OrderDetailsCreateDTO> OrderDetailsDTO { get; set; } = new();
    }
}
