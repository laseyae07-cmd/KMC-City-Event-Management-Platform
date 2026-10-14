using System.ComponentModel.DataAnnotations;

namespace KMCEventPlatform.API.Models
{
    public class CityEvent
    {
        [Key]
        public int EventId { get; set; }

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

        [Required]
        [MaxLength(30)]
        public string Status { get; set; } = "Upcoming";

        public int EventTypeId { get; set; }
        public EventType? EventType { get; set; }

        public int OrganizerId { get; set; }
        public User? Organizer { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        public ICollection<Registration> Registrations { get; set; }
            = new List<Registration>();
    }
}