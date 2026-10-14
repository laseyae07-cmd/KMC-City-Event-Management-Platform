using System.ComponentModel.DataAnnotations;

namespace KMCEventPlatform.API.DTOs
{
    public class CreateRegistrationDto
    {
        [Range(1, int.MaxValue)]
        public int EventId { get; set; }
    }
}