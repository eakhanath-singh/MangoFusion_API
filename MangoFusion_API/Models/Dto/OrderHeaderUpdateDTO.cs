using System.ComponentModel.DataAnnotations;

namespace MangoFusion_API.Models.Dto
{
    public class OrderHeaderUpdateDTO
    {
        [Required]
        public int OrderHeaderId { get; set; }
        public string PickupName { get; set; } = string.Empty;
        public string PickupPhoneNumber { get; set; } = string.Empty;
        public string PickupEmail { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}
