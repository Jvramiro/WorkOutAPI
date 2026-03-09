namespace WorkOutAPI.Models
{
    public class CheckIn
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public required User User { get; set; }
        public DateTime Date { get; set; }
    }
}