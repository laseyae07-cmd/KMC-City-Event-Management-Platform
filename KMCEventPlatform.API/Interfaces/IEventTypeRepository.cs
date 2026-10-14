using KMCEventPlatform.API.Models;

namespace KMCEventPlatform.API.Interfaces
{
    public interface IEventTypeRepository
    {
        Task<IEnumerable<EventType>> GetAllAsync();

        Task<EventType?> GetByIdAsync(int id);
    }
}