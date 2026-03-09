using WorkOutAPI.Enums;

namespace WorkOutAPI.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public Role Role { get; set; }
        public ICollection<Exercise> Schedule { get; set; } = new List<Exercise>();
        public string RefreshToken { get; set; } = string.Empty;
    }
}