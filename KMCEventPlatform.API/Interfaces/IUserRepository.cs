using KMCEventPlatform.API.Models;

namespace KMCEventPlatform.API.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);

        Task AddAsync(User user);

        Task SaveChangesAsync();
    }
}