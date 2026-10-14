using System.ComponentModel.DataAnnotations;

namespace KMCEventPlatform.API.Models
{
    public class EventType
    {
        public int EventTypeId { get; set; }

        [Required]
        [MaxLength(100)]
        public string TypeName { get; set; } = string.Empty;

        [MaxLength(300)]
        public string? Description { get; set; }

        public ICollection<CityEvent> Events { get; set; }
            = new List<CityEvent>();
    }
}