namespace KMCEventPlatform.Client.Models
{
    public class EventTypeViewModel
    {
        public int EventTypeId { get; set; }

        public string TypeName { get; set; } = string.Empty;

        public string? Description { get; set; }
    }
}