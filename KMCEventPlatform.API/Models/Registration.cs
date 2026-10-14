using System.ComponentModel.DataAnnotations;

namespace KMCEventPlatform.API.Models
{
    public class Registration
    {
        public int RegistrationId { get; set; }

        // Foreign key for the selected event.
        public int EventId { get; set; }

        public CityEvent? Event { get; set; }

        // Foreign key for the participant.
        public int ParticipantId { get; set; }

        public User? Participant { get; set; }

        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Registered";
    }
}