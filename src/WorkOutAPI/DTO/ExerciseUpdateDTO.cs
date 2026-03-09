using WorkOutAPI.Enums;

namespace WorkOutAPI.DTO
{
    public record ExerciseUpdateDTO(string? Name, MuscleGroup? Group);
}