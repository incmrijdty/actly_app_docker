namespace Actly.API.DTO
{
    public class UserDto
    {
        public int Id { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
        public List<ParticipationDto>? Participations { get; set; }
    }
}