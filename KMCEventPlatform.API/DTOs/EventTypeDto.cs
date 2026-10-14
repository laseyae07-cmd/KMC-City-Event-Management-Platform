namespace KMCEventPlatform.API.DTOs
{
    public class EventTypeDto
    {
        public int EventTypeId { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}