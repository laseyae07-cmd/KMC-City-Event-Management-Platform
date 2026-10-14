namespace KMCEventPlatform.Client.Models
{
    public class AuthServiceResult
    {
        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public AuthResponseViewModel? Response { get; set; }
    }
}