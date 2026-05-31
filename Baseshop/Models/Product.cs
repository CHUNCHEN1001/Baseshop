using System.ComponentModel.DataAnnotations;
using Baseshop.Enums; // 引用你定義 Enum 的命名空間

namespace Baseshop.Models
{
    public class Product
    {
        [Key]
        public string ProductId { get; set; }
        
        [Required]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; }

        [Required]
        public int StockQuantity { get; set; }

        [Required]
        public ProductStatus Status { get; set; }
        public string? ImagePath { get; set; }

        public DateTime CreatedTime { get; set; } = DateTime.Now;
        public string? LastUpdatedBy { get; set; }
        public DateTime? LastUpdatedTime { get; set; }
    }

}
