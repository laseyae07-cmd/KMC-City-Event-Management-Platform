namespace KMCEventPlatform.Client.Models
{
    public class EventViewModel
    {
        public int EventId { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public TimeSpan StartTime { get; set; }

        public TimeSpan EndTime { get; set; }

        public string Venue { get; set; } = string.Empty;

        public int MaximumParticipants { get; set; }

        public int RegisteredParticipants { get; set; }

        public string Status { get; set; } = string.Empty;

        public int EventTypeId { get; set; }

        public string EventTypeName { get; set; } = string.Empty;

        public int OrganizerId { get; set; }

        public string OrganizerName { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}