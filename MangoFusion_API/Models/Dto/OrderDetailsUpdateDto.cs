using System.ComponentModel.DataAnnotations;

namespace MangoFusion_API.Models.Dto
{
    public class OrderDetailsUpdateDto
    {
        [Required]
        public int OrderDetailsId { get; set; }
        [Range(1,5)]
        [Required]
        public int Rating { get; set; }
    }
}
