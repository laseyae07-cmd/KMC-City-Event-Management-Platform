using KMCEventPlatform.API.DTOs;
using KMCEventPlatform.API.Models;

namespace KMCEventPlatform.API.Interfaces
{
    public interface IEventRepository
    {
        Task<IEnumerable<CityEvent>> GetAllAsync();

        Task<CityEvent?> GetByIdAsync(
            int id,
            bool trackChanges = false
        );

        Task<IEnumerable<CityEvent>>
            GetByOrganizerIdAsync(int organizerId);

        Task<IEnumerable<CityEvent>> SearchAsync(
            EventSearchDto searchDto
        );

        Task<bool> EventTypeExistsAsync(int eventTypeId);

        Task AddAsync(CityEvent cityEvent);

        void Delete(CityEvent cityEvent);

        Task SaveChangesAsync();
    }
}