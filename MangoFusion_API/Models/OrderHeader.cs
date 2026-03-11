using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusion_API.Models
{
    public class OrderHeader
    {
        [Key]
        public int OrderHeaderId { get; set; }
        [Required]
        public string PickupName { get; set; } = string.Empty;
        [Required]
        public string PickupPhoneNumber { get; set; } = string.Empty;
        [Required]
        public string PickupEmail { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        /// <summary>
        /// Adding a foreign key from table application User
        /// </summary>
        public string ApplicationUserId { get; set; } = string.Empty;
        [ForeignKey("ApplciationUserId")]
        public ApplicationUser? ApplicationUser { get; set; }
        public double OrderTotal { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TotalItem { get; set; }

        public List<OrderDetails> OrderDetails { get; set; } = new();   

    }
}
