using WorkOutAPI.Enums;

namespace WorkOutAPI.DTO
{
    public record ExerciseCreateDTO(string Name, MuscleGroup Group);
}