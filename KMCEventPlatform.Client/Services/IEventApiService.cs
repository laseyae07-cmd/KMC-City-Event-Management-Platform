using KMCEventPlatform.Client.Models;

namespace KMCEventPlatform.Client.Services
{
    public interface IEventApiService
    {
        Task<List<EventViewModel>> GetEventsAsync();

        Task<EventViewModel?> GetEventByIdAsync(int id);

        Task<List<EventViewModel>> SearchEventsAsync(
            string? title,
            DateTime? date,
            int? eventTypeId,
            string? venue,
            string? status
        );

        Task<List<EventTypeViewModel>> GetEventTypesAsync();
    }
}