using System.ComponentModel.DataAnnotations;
using Baseshop.Dtos;
using Baseshop.Models;

namespace Baseshop.ValidationAttributes
{
    public class AccountAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            WebContext _webContext = (WebContext)validationContext.GetService(typeof(WebContext));

            var account = (string)value;

            var findAccount = from a in _webContext.Users
                            where a.Account == account
                            select a;

            var dto = validationContext.ObjectInstance;

            if (dto.GetType() == typeof(UsersEditDto))
            {
                var updateDto = (UsersEditDto)dto;
                findAccount = findAccount.Where(a => a.UserName != updateDto.UserName);
            }

            if (findAccount.FirstOrDefault() != null)
            {
                return new ValidationResult("已存在相同的帳號");
            }

            return ValidationResult.Success;
        }
    }
}
