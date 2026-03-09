using WorkOutAPI.Enums;
using WorkOutAPI.Models;

namespace WorkOutAPI.DTO
{
    public record UserGetDTO(int Id, string Username, string Email, Role Role, ICollection<Exercise> Schedule);
}