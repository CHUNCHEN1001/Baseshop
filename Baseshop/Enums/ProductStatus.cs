using System.ComponentModel.DataAnnotations;

namespace Baseshop.Enums
{
    public enum ProductStatus
    {
        [Display(Name = "可銷售")]
        Available,

        [Display(Name = "凍結")]
        Frozen,

        [Display(Name = "數量不足")]
        LowStock
    }
}
