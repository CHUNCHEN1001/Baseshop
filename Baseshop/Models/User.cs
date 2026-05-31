using System.ComponentModel.DataAnnotations;

namespace Baseshop.Models
{
    public class User
    {
        [Key]
        public string Account { get; set; }
        
        [Required]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; } // 加密後的密碼

        // 暫時用 string 存權限，例如 "Admin"
        public string Role { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.Now;

        [MaxLength(50)]
        public string? LastUpdatedBy { get; set; }

        public DateTime? LastUpdatedTime { get; set; }
    }

}
