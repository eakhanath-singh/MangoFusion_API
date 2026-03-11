using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MangoFusion_API.Models.Dto
{
    public class MenuItemCreateDTO
    {
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
        public string category { get; set; }
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
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
