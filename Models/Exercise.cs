using WorkOutAPI.Enums;

namespace WorkOutAPI.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public MuscleGroup Group { get; set; }
    }
}