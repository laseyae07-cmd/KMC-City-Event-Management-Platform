using KMCEventPlatform.API.Models;

namespace KMCEventPlatform.API.Interfaces
{
    public interface IRegistrationRepository
    {
        Task<Registration?> GetByIdAsync(
            int registrationId,
            bool trackChanges = false
        );

        Task<Registration?> GetByEventAndParticipantAsync(
            int eventId,
            int participantId,
            bool trackChanges = false
        );

        Task<IEnumerable<Registration>>
            GetByParticipantIdAsync(int participantId);

        Task<IEnumerable<Registration>>
            GetByEventIdAsync(int eventId);

        Task<int> CountActiveRegistrationsAsync(int eventId);

        Task AddAsync(Registration registration);

        Task SaveChangesAsync();
    }
}