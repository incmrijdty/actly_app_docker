using System.ComponentModel.DataAnnotations;

namespace Actly.API.Models
{
    public class Event
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Title { get; set; }

        public int? OrganizerId { get; set; }

        public User? Organizer { get; set; } = null!;

        [Required]
        public required string Description { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public required string Location { get; set; }

        [Required]
        public int MaxParticipants { get; set; }

        [Required]
        public required string Category { get; set; }

        public required List<Participation> Participations { get; set; }
    }
}