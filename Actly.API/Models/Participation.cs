using System.ComponentModel.DataAnnotations;

namespace Actly.API.Models
{
    public class Participation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public required User? User { get; set; }

        [Required]
        public int EventId { get; set; }

        public required Event? Event { get; set; }
        public bool Attended { get; set; }

        [Required]
        public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
    }
}