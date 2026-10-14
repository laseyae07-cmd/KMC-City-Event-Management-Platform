namespace KMCEventPlatform.Client.Models
{
    public class OrganizerEventServiceResult
    {
        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public EventViewModel? Event { get; set; }
    }
}