using System.ComponentModel.DataAnnotations;

namespace WorkOutAPI.DTO
{
    public class CheckInCreateDTO
    {
        public DateTime Date { get; set; } = DateTime.Now;
    }
}