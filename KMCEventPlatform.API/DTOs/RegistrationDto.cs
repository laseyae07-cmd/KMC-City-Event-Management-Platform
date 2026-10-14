namespace KMCEventPlatform.API.DTOs
{
    public class RegistrationDto
    {
        public int RegistrationId { get; set; }

        public int EventId { get; set; }

        public string EventTitle { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Venue { get; set; } = string.Empty;

        public int ParticipantId { get; set; }

        public string ParticipantName { get; set; } = string.Empty;

        public string ParticipantEmail { get; set; } = string.Empty;

        public DateTime RegistrationDate { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}