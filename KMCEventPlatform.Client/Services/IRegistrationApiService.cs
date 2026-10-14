using KMCEventPlatform.Client.Models;

namespace KMCEventPlatform.Client.Services
{
    public interface IRegistrationApiService
    {
        Task<RegistrationServiceResult> RegisterForEventAsync(
            int eventId,
            string token
        );

        Task<List<RegistrationViewModel>>
            GetMyRegistrationsAsync(string token);

        Task<ApiActionResult> CancelRegistrationAsync(
            int registrationId,
            string token
        );
    }
}