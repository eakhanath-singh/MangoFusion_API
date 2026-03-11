using System.ComponentModel.DataAnnotations;

namespace MangoFusion_API.Models.Dto
{
    public class MenuItemUpdateDTO
    {
        [Key]
        public int id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        public string name { get; set; }
        /// <summary>
        /// Description
        /// </summary>
        public string? description { get; set; }
        /// <summary>
        /// Category
        /// </summary>
        public string category { get; set; }= string.Empty;
        /// <summary>
        /// Special Tag
        /// </summary>
        public string? specialTag { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        [Range(1, 1000)]
        public double price { get; set; }
        /// <summary>
        /// Image
        /// </summary>
        public IFormFile? File { get; set; }
    }
}
