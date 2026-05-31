using System.ComponentModel;
using Baseshop.Models;
using System.ComponentModel.DataAnnotations;
using Baseshop.ValidationAttributes;

namespace Baseshop.Dtos
{
    public class UsersDto
    {
        [DisplayName("帳號")]
        [Account]
        public string Account { get; set; }

        [DisplayName("使用者名稱")]
        public string UserName { get; set; }


        [DisplayName("Email")]
        public string Email { get; set; }

        [DisplayName("密碼")]
        public string Password { get; set; }

        [DisplayName("權限")]
        public string Role { get; set; }

        [DisplayName("最後修改時間")]
        public DateTime? LastUpdatedTime { get; set; }

        [DisplayName("最後修改人員")]
        public string? LastUpdatedBy { get; set; }
    }
}
