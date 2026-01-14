using System.ComponentModel.DataAnnotations;

namespace Actly.API.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public required string Username { get; set; }

        [Required, EmailAddress]
        public required string Email { get; set; }

        [Required]
        public required string Role { get; set; } 

        [Required]
        public required string PasswordHash { get; set; }

        public required List<Participation> Participations { get; set; }
    }
}
