namespace KMCEventPlatform.Client.Models
{
    public class EventSearchViewModel
    {
        public string? Title { get; set; }

        public DateTime? Date { get; set; }

        public int? EventTypeId { get; set; }

        public string? Venue { get; set; }

        public string? Status { get; set; }

        public List<EventViewModel> Events { get; set; }
            = new List<EventViewModel>();

        public List<EventTypeViewModel> EventTypes { get; set; }
            = new List<EventTypeViewModel>();
    }
}