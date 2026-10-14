using KMCEventPlatform.API.Data;
using KMCEventPlatform.API.Interfaces;
using KMCEventPlatform.API.Models;
using Microsoft.EntityFrameworkCore;

namespace KMCEventPlatform.API.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            string normalizedEmail = email.Trim().ToLowerInvariant();

            return await _context.Users.FirstOrDefaultAsync(
                user => user.Email == normalizedEmail
            );
        }

        public async Task AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}