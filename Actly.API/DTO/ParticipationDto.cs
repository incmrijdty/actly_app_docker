namespace Actly.API.DTO
{
    public class ParticipationDto
    {
        public int Id { get; set; }
        public required int UserId { get; set; }
        public required int EventId { get; set; }
        public required bool Attended { get; set; }
        public UserDto? User { get; set; }
        public EventBriefDto? Event { get; set; }
    }
}
