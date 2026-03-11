using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MangoFusion_API.Models
{
    public class MenuItem
    {
        /// <summary>
        /// Id
        /// </summary>
        [Key]
        public int id { get; set; }
        /// <summary>
        /// Name
        /// </summary>
        [Required]
        public string name { get; set; } = string.Empty;
        /// <summary>
        /// Description
        /// </summary>
        public string? description { get; set; }
        /// <summary>
        /// Category
        /// </summary>
        public string category { get; set; } = string.Empty;
        /// <summary>
        /// Special Tag
        /// </summary>
        public string? specialTag { get; set; }
        /// <summary>
        /// Price
        /// </summary>
        [Range(1,1000)]
        public double price { get; set; }
        /// <summary>
        /// Image
        /// </summary>
        [Required]
        public string image { get; set; } = string.Empty;
        /// <summary>
        /// Rating with not mapped attribute meaning it will not be created in database
        /// </summary>

        [NotMapped]
        public double? Rating { get; set; }
    }
}
