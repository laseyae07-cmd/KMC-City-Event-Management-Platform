using System.ComponentModel.DataAnnotations;

namespace KMCEventPlatform.API.DTOs
{
    public class EventSearchDto
    {
        [MaxLength(150)]
        public string? Title { get; set; }

        public DateTime? Date { get; set; }

        [Range(1, int.MaxValue)]
        public int? EventTypeId { get; set; }

        [MaxLength(250)]
        public string? Venue { get; set; }

        [RegularExpression(
            "^(Upcoming|Cancelled|Completed)$",
            ErrorMessage =
                "Status must be Upcoming, Cancelled or Completed."
        )]
        public string? Status { get; set; }
    }
}