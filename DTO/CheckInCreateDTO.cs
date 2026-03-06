using System.ComponentModel.DataAnnotations;

namespace WorkOutAPI.DTO
{
    public class CheckInCreateDTO
    {
        [Required]
        public int UserId { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}