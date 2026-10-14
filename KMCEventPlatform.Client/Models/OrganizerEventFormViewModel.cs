using System.ComponentModel.DataAnnotations;

namespace KMCEventPlatform.Client.Models
{
    public class OrganizerEventFormViewModel
    {
        public int EventId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Event date")]
        [DataType(DataType.Date)]
        public DateTime EventDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Start time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [MaxLength(250)]
        public string Venue { get; set; } = string.Empty;

        [Range(1, 100000)]
        [Display(Name = "Maximum participants")]
        public int MaximumParticipants { get; set; }

        [Range(1, int.MaxValue)]
        [Display(Name = "Event type")]
        public int EventTypeId { get; set; }

        [Required]
        [RegularExpression(
            "^(Upcoming|Cancelled|Completed)$",
            ErrorMessage =
                "Status must be Upcoming, Cancelled or Completed."
        )]
        public string Status { get; set; } = "Upcoming";

        public List<EventTypeViewModel> EventTypes { get; set; }
            = new List<EventTypeViewModel>();
    }
}