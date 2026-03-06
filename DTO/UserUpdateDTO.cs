using WorkOutAPI.Models;

namespace WorkOutAPI.DTO
{
    public record UserUpdateDTO(string? Username, ICollection<Exercise>? Schedule);
}