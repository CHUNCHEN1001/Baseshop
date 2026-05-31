using System.ComponentModel;
using Baseshop.Models;
using System.ComponentModel.DataAnnotations;
using Baseshop.ValidationAttributes;

namespace Baseshop.Dtos
{
    public class UsersCreateDto
    {
        [Required(ErrorMessage = "帳號為必填欄位")]
        [DisplayName("帳號")]
        [Account]
        public string Account { get; set; }

        [Required(ErrorMessage = "使用者名稱為必填欄位")]
        [DisplayName("使用者名稱")]
        public string UserName { get; set; }


        [Required(ErrorMessage = "電子郵件為必填欄位")]
        [EmailAddress(ErrorMessage = "電子郵件格式不正確")]
        [DisplayName("Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "密碼為必填欄位")]
        [DisplayName("密碼")]
        public string Password { get; set; }

        // 暫時用 string 存權限，例如 "Admin"
        [Required(ErrorMessage = "權限為必填欄位")]
        [DisplayName("權限")]
        public string Role { get; set; }

    }
}
