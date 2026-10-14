using System.ComponentModel.DataAnnotations;

namespace KMCEventPlatform.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(150)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [MaxLength(100)]
        public string Password { get; set; } = string.Empty;

        [MaxLength(20)]
        public string? PhoneNumber { get; set; }

        [Required]
        [RegularExpression(
            "^(Organizer|Participant)$",
            ErrorMessage = "Role must be Organizer or Participant."
        )]
        public string Role { get; set; } = string.Empty;
    }
}