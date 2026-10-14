namespace KMCEventPlatform.Client.Models
{
    public class ApiActionResult
    {
        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;
    }
}