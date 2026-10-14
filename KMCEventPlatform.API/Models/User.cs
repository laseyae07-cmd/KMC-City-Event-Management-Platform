using System.ComponentModel.DataAnnotations;

namespace KMCEventPlatform.API.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MaxLength(500)]
        public string PasswordHash { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [MaxLength(20)]
        public string Role { get; set; } = "Participant";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Events created by this user when acting as an organizer.
        public ICollection<CityEvent> CreatedEvents { get; set; }
            = new List<CityEvent>();

        // Events for which this user has registered.
        public ICollection<Registration> Registrations { get; set; }
            = new List<Registration>();
    }
}