using System.ComponentModel.DataAnnotations;

namespace ThuVien_API.Models.DTO
{
    public class LoginRequestDTO
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
