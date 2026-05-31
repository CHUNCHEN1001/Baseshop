using Baseshop.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Baseshop.Dtos
{
    public class ProductsDto
    {
        [DisplayName("商品代碼")]
        public string ProductId { get; set; }

        [DisplayName("品名")]
        public string Name { get; set; }

        [DisplayName("商品類別")]
        public string Category { get; set; }

        [DisplayName("庫存數量")]
        public int StockQuantity { get; set; }

        [DisplayName("商品狀態")]
        public ProductStatus Status { get; set; }

        [DisplayName("商品圖片路徑")]
        public string? ImagePath { get; set; }

        [DisplayName("新增時間")]
        public DateTime CreatedTime { get; set; }

        [DisplayName("最後修改人員")]
        public string? LastUpdatedBy { get; set; }

        [DisplayName("最後修改時間")]
        public DateTime? LastUpdatedTime { get; set; }
    }
}
