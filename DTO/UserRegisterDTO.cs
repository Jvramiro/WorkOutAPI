using System.ComponentModel.DataAnnotations;
using WorkOutAPI.Models;

namespace WorkOutAPI.DTO
{
    public class UserRegisterDTO
    {
        [Required]
        [MinLength(3), MaxLength(100)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        public string Password { get; set; } = string.Empty;
    }
}