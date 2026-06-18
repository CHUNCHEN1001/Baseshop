using Baseshop.Enums;
using Baseshop.ValidationAttributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Baseshop.Dtos
{
    public class ProductsEditDto
    {
        [Required(ErrorMessage = "商品代碼為必填欄位")]
        [DisplayName("商品代碼")]
        public string ProductId { get; set; }

        [Required(ErrorMessage = "品名為必填欄位")]
        [DisplayName("品名")]
        [Product]
        public string Name { get; set; }

        [Required(ErrorMessage = "商品類別為必填欄位")]
        [DisplayName("商品類別")]
        public string Category { get; set; }

        [Required(ErrorMessage = "庫存數量為必填欄位")]
        [DisplayName("庫存數量")]
        public int StockQuantity { get; set; }

        [Required(ErrorMessage = "商品狀態為必填欄位")]
        [DisplayName("商品狀態")]
        public ProductStatus Status { get; set; }

        [DisplayName("商品圖片路徑")]
        public string? ImagePath { get; set; }
    }

}
