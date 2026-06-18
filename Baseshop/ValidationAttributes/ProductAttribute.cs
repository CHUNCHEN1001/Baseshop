using System.ComponentModel.DataAnnotations;
using Baseshop.Dtos;
using Baseshop.Models;

namespace Baseshop.ValidationAttributes
{
    public class ProductAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            WebContext _webContext = (WebContext)validationContext.GetService(typeof(WebContext));

            var productId = (string)value;

            var findProduct = from a in _webContext.Products
                              where a.ProductId == productId
                              select a;

            var dto = validationContext.ObjectInstance;

            if (dto.GetType() == typeof(ProductsEditDto))
            {
                var updateDto = (ProductsEditDto)dto;
                findProduct = findProduct.Where(a => a.Name != updateDto.Name);
                return new ValidationResult("已存在相同的商品名稱");
            }

            if (findProduct.FirstOrDefault() != null)
            {
                return new ValidationResult("已存在相同的商品");
            }

            return ValidationResult.Success;
        }
    }
}
