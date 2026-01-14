namespace Actly.API.DTO
{
    public class CreateEventDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required DateTime Date { get; set; }
        public required string Location { get; set; }
        public required int MaxParticipants { get; set; }
        public required string Category { get; set; }
    }
}
