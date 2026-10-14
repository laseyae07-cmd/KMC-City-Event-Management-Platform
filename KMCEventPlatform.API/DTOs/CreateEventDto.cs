using System.ComponentModel.DataAnnotations;

namespace KMCEventPlatform.API.DTOs
{
    public class CreateEventDto
    {
        [Required]
        [MaxLength(150)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [MaxLength(2000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public DateTime EventDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [MaxLength(250)]
        public string Venue { get; set; } = string.Empty;

        [Range(1, 100000)]
        public int MaximumParticipants { get; set; }

        [Range(1, int.MaxValue)]
        public int EventTypeId { get; set; }
    }
}