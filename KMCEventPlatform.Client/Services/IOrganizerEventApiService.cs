using KMCEventPlatform.Client.Models;

namespace KMCEventPlatform.Client.Services
{
    public interface IOrganizerEventApiService
    {
        Task<List<EventViewModel>> GetMyEventsAsync(
            string token
        );

        Task<List<RegistrationViewModel>>
            GetEventRegistrationsAsync(
                int eventId,
                string token
            );

        Task<OrganizerEventServiceResult> CreateEventAsync(
            OrganizerEventFormViewModel eventViewModel,
            string token
        );

        Task<OrganizerEventServiceResult> UpdateEventAsync(
            int eventId,
            OrganizerEventFormViewModel eventViewModel,
            string token
        );

        Task<ApiActionResult> DeleteEventAsync(
            int eventId,
            string token
        );
    }
}