namespace KMCEventPlatform.Client.Models
{
    public class RegistrationServiceResult
    {
        public bool Succeeded { get; set; }

        public string ErrorMessage { get; set; } = string.Empty;

        public RegistrationViewModel? Registration { get; set; }
    }
}